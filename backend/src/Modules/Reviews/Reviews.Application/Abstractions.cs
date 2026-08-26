using JewelryStore.Modules.Reviews.Domain;

namespace JewelryStore.Modules.Reviews.Application;

public interface IReviewRepository
{
    Task Add(Review review, CancellationToken cancellationToken);
    Task<Review?> Get(Guid id, CancellationToken cancellationToken);
}

public interface IReviewUnitOfWork
{
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public interface IReviewReader
{
    Task<ReviewSummaryDto> ForProduct(Guid productId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReviewDto>> Pending(CancellationToken cancellationToken);
}

