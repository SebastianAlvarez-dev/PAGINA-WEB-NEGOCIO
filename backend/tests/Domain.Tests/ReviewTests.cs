using JewelryStore.BuildingBlocks.Domain;
using JewelryStore.Modules.Reviews.Domain;
using Xunit;

namespace JewelryStore.Domain.Tests;

public sealed class ReviewTests
{
    [Fact]
    public void Create_StartsPendingAndCanBeApproved()
    {
        var now = DateTimeOffset.UtcNow;
        var review = Review.Create(Guid.NewGuid(), "María", "Me encantó la pieza", 5, now);

        Assert.Equal(ReviewStatus.Pending, review.Status);

        review.Moderate(true, now.AddMinutes(1));

        Assert.Equal(ReviewStatus.Approved, review.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_RejectsRatingOutsideRange(int rating)
        => Assert.Throws<DomainException>(() =>
            Review.Create(Guid.NewGuid(), "Ana", "Comentario válido", rating, DateTimeOffset.UtcNow));
}
