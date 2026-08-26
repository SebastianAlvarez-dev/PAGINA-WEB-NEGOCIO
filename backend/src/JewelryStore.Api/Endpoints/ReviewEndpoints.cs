using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Reviews.Application;

namespace JewelryStore.Api.Endpoints;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var reviews = endpoints.MapGroup("/api/products/{productId:guid}/reviews").WithTags("Reseñas");

        reviews.MapGet("/", async (
            Guid productId,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
            Results.Ok(await dispatcher.Query<GetProductReviewsQuery, ReviewSummaryDto>(
                new GetProductReviewsQuery(productId), cancellationToken)));

        reviews.MapPost("/", async (
            Guid productId,
            SubmitReviewRequest request,
            MessageDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var id = await dispatcher.Send<SubmitReviewCommand, Guid>(
                new SubmitReviewCommand(productId, request.AuthorName, request.Comment, request.Rating),
                cancellationToken);
            return Results.Accepted(value: new
            {
                id,
                message = "Gracias. Tu reseña será visible después de ser revisada."
            });
        }).RequireRateLimiting("reviews");

        return endpoints;
    }
}

public sealed record SubmitReviewRequest(string AuthorName, string Comment, int Rating);

