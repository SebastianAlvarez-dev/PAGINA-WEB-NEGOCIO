using JewelryStore.Modules.Reviews.Application;
using JewelryStore.Modules.Reviews.Domain;
using Microsoft.EntityFrameworkCore;

namespace JewelryStore.Modules.Reviews.Infrastructure;

public sealed class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options)
    : DbContext(options), IReviewRepository, IReviewUnitOfWork, IReviewReader
{
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reviews");
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("reviews");
            entity.HasKey(review => review.Id);
            entity.Property(review => review.AuthorName).HasMaxLength(80).IsRequired();
            entity.Property(review => review.Comment).HasMaxLength(1_000).IsRequired();
            entity.Property(review => review.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(review => new { review.ProductId, review.Status, review.CreatedAt });
        });
    }

    Task IReviewRepository.Add(Review review, CancellationToken cancellationToken)
        => Reviews.AddAsync(review, cancellationToken).AsTask();

    Task<Review?> IReviewRepository.Get(Guid id, CancellationToken cancellationToken)
        => Reviews.SingleOrDefaultAsync(review => review.Id == id, cancellationToken);

    Task<int> IReviewUnitOfWork.SaveChanges(CancellationToken cancellationToken)
        => SaveChangesAsync(cancellationToken);

    async Task<ReviewSummaryDto> IReviewReader.ForProduct(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var reviews = await Reviews
            .AsNoTracking()
            .Where(review =>
                review.ProductId == productId &&
                review.Status == ReviewStatus.Approved)
            .OrderByDescending(review => review.CreatedAt)
            .Select(review => Map(review))
            .ToListAsync(cancellationToken);

        return new ReviewSummaryDto(
            productId,
            reviews.Count == 0 ? 0 : Math.Round(reviews.Average(review => review.Rating), 1),
            reviews.Count,
            reviews);
    }

    async Task<IReadOnlyList<ReviewDto>> IReviewReader.Pending(CancellationToken cancellationToken)
        => await Reviews
            .AsNoTracking()
            .Where(review => review.Status == ReviewStatus.Pending)
            .OrderBy(review => review.CreatedAt)
            .Select(review => Map(review))
            .ToListAsync(cancellationToken);

    private static ReviewDto Map(Review review)
        => new(
            review.Id,
            review.ProductId,
            review.AuthorName,
            review.Comment,
            review.Rating,
            review.Status.ToString(),
            review.CreatedAt);
}

