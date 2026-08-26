using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Reviews.Domain;

namespace JewelryStore.Modules.Reviews.Application;

public sealed record SubmitReviewCommand(
    Guid ProductId,
    string AuthorName,
    string Comment,
    int Rating) : ICommand<Guid>;

public sealed class SubmitReviewHandler(
    IProductExistence products,
    IReviewRepository reviews,
    IReviewUnitOfWork unitOfWork) : ICommandHandler<SubmitReviewCommand, Guid>
{
    public async Task<Guid> Handle(SubmitReviewCommand command, CancellationToken cancellationToken)
    {
        if (!await products.Exists(command.ProductId, cancellationToken))
        {
            throw new KeyNotFoundException("El producto no existe.");
        }

        var review = Review.Create(
            command.ProductId,
            command.AuthorName,
            command.Comment,
            command.Rating,
            DateTimeOffset.UtcNow);

        await reviews.Add(review, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);
        return review.Id;
    }
}

public sealed record ModerateReviewCommand(Guid ReviewId, bool Approve) : ICommand<bool>;

public sealed class ModerateReviewHandler(
    IReviewRepository reviews,
    IReviewUnitOfWork unitOfWork) : ICommandHandler<ModerateReviewCommand, bool>
{
    public async Task<bool> Handle(ModerateReviewCommand command, CancellationToken cancellationToken)
    {
        var review = await reviews.Get(command.ReviewId, cancellationToken)
            ?? throw new KeyNotFoundException("La reseña no existe.");

        review.Moderate(command.Approve, DateTimeOffset.UtcNow);
        await unitOfWork.SaveChanges(cancellationToken);
        return true;
    }
}

