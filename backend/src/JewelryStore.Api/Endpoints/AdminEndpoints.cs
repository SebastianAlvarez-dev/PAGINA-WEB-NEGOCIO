using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Catalog.Application;
using JewelryStore.Modules.Reviews.Application;
using Microsoft.AspNetCore.Mvc;

namespace JewelryStore.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var admin = endpoints.MapGroup("/api/admin")
            .RequireAuthorization("admin")
            .WithTags("Administración");

        admin.MapGet("/products", async (
            [FromQuery] string? search,
            [FromQuery] Guid? categoryId,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
            Results.Ok(await dispatcher.Query<GetProductsQuery, PagedProducts>(
                new GetProductsQuery(new ProductFilter(search, categoryId, true, 1, 100)), cancellationToken)));

        admin.MapGet("/products/{id:guid}", async (
            Guid id,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var product = await dispatcher.Query<GetProductByIdQuery, ProductDetailsDto?>(
                new GetProductByIdQuery(id), cancellationToken);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        admin.MapPost("/categories", async (
            CreateCategoryRequest request,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var id = await dispatcher.Send<CreateCategoryCommand, Guid>(
                new CreateCategoryCommand(request.Name), cancellationToken);
            return Results.Created($"/api/catalog/categories/{id}", new { id });
        });

        admin.MapPost("/products", async (
            SaveProductRequest request,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var id = await dispatcher.Send<CreateProductCommand, Guid>(
                new CreateProductCommand(
                    request.Name,
                    request.Description,
                    request.CategoryId,
                    request.Price,
                    request.Stock,
                    request.ImageUrl),
                cancellationToken);
            return Results.Created($"/api/admin/products/{id}", new { id });
        });

        admin.MapPut("/products/{id:guid}", async (
            Guid id,
            SaveProductRequest request,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            await dispatcher.Send<UpdateProductCommand, bool>(
                new UpdateProductCommand(
                    id,
                    request.Name,
                    request.Description,
                    request.CategoryId,
                    request.Price,
                    request.Stock,
                    request.ImageUrl,
                    request.IsActive),
                cancellationToken);
            return Results.NoContent();
        });

        admin.MapDelete("/products/{id:guid}", async (
            Guid id,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            await dispatcher.Send<ArchiveProductCommand, bool>(
                new ArchiveProductCommand(id), cancellationToken);
            return Results.NoContent();
        });

        admin.MapPost("/images", async (
            IFormFile file,
            IImageStorage storage,
            CancellationToken cancellationToken) =>
        {
            if (file.Length is 0 or > 8_388_608)
            {
                return Results.BadRequest(new { message = "La imagen debe pesar entre 1 byte y 8 MB." });
            }

            await using var stream = file.OpenReadStream();
            var url = await storage.UploadProductImage(
                stream, file.FileName, file.ContentType, cancellationToken);
            return Results.Ok(new { url });
        }).DisableAntiforgery();

        admin.MapGet("/reviews/pending", async (
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
            Results.Ok(await dispatcher.Query<GetPendingReviewsQuery, IReadOnlyList<ReviewDto>>(
                new GetPendingReviewsQuery(), cancellationToken)));

        admin.MapPut("/reviews/{id:guid}/moderation", async (
            Guid id,
            ModerateReviewRequest request,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            await dispatcher.Send<ModerateReviewCommand, bool>(
                new ModerateReviewCommand(id, request.Approve), cancellationToken);
            return Results.NoContent();
        });

        return endpoints;
    }
}

public sealed record CreateCategoryRequest(string Name);
public sealed record SaveProductRequest(
    string Name,
    string Description,
    Guid CategoryId,
    decimal Price,
    int Stock,
    string? ImageUrl,
    bool IsActive = true);
public sealed record ModerateReviewRequest(bool Approve);

