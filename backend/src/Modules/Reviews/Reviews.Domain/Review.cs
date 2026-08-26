using JewelryStore.BuildingBlocks.Domain;

namespace JewelryStore.Modules.Reviews.Domain;

public enum ReviewStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public sealed class Review : AggregateRoot
{
    private Review() { }

    private Review(
        Guid id,
        Guid productId,
        string authorName,
        string comment,
        int rating,
        DateTimeOffset now)
    {
        Id = id;
        ProductId = productId;
        AuthorName = authorName;
        Comment = comment;
        Rating = rating;
        Status = ReviewStatus.Pending;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public string Comment { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public ReviewStatus Status { get; private set; }

    public static Review Create(
        Guid productId,
        string authorName,
        string comment,
        int rating,
        DateTimeOffset now)
        => new(
            Guid.NewGuid(),
            productId,
            GuardAuthor(authorName),
            GuardComment(comment),
            GuardRating(rating),
            now);

    public void Moderate(bool approve, DateTimeOffset now)
    {
        Status = approve ? ReviewStatus.Approved : ReviewStatus.Rejected;
        Touch(now);
    }

    private static string GuardAuthor(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is < 2 or > 80)
        {
            throw new DomainException("El nombre debe tener entre 2 y 80 caracteres.");
        }

        return normalized;
    }

    private static string GuardComment(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is < 5 or > 1_000)
        {
            throw new DomainException("El comentario debe tener entre 5 y 1000 caracteres.");
        }

        return normalized;
    }

    private static int GuardRating(int value)
    {
        if (value is < 1 or > 5)
        {
            throw new DomainException("La puntuación debe estar entre 1 y 5.");
        }

        return value;
    }
}
