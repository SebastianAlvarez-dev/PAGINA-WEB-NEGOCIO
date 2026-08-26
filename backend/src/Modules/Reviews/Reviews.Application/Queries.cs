using JewelryStore.BuildingBlocks.Application;

namespace JewelryStore.Modules.Reviews.Application;

public sealed record GetProductReviewsQuery(Guid ProductId) : IQuery<ReviewSummaryDto>;

public sealed class GetProductReviewsHandler(IReviewReader reader)
    : IQueryHandler<GetProductReviewsQuery, ReviewSummaryDto>
{
    public Task<ReviewSummaryDto> Handle(
        GetProductReviewsQuery query,
        CancellationToken cancellationToken)
        => reader.ForProduct(query.ProductId, cancellationToken);
}

public sealed record GetPendingReviewsQuery : IQuery<IReadOnlyList<ReviewDto>>;

public sealed class GetPendingReviewsHandler(IReviewReader reader)
    : IQueryHandler<GetPendingReviewsQuery, IReadOnlyList<ReviewDto>>
{
    public Task<IReadOnlyList<ReviewDto>> Handle(
        GetPendingReviewsQuery query,
        CancellationToken cancellationToken)
        => reader.Pending(cancellationToken);
}
