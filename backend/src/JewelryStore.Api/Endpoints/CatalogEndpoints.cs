using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Catalog.Application;
using Microsoft.AspNetCore.Mvc;

namespace JewelryStore.Api.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var catalog = endpoints.MapGroup("/api/catalog").WithTags("Catálogo");

        catalog.MapGet("/categories", async (MessageDispatcher dispatcher, CancellationToken cancellationToken) =>
            Results.Ok(await dispatcher.Query<GetCategoriesQuery, IReadOnlyList<CategoryDto>>(
                new GetCategoriesQuery(), cancellationToken)));

        catalog.MapGet("/products", async (
            [FromQuery] string? search,
            [FromQuery] Guid? categoryId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.Query<GetProductsQuery, PagedProducts>(
                new GetProductsQuery(new ProductFilter(search, categoryId, false, page, pageSize == 0 ? 24 : pageSize)),
                cancellationToken);
            return Results.Ok(result);
        });

        catalog.MapGet("/products/{slug}", async (
            string slug,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var product = await dispatcher.Query<GetProductBySlugQuery, ProductDetailsDto?>(
                new GetProductBySlugQuery(slug), cancellationToken);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        return endpoints;
    }
}

