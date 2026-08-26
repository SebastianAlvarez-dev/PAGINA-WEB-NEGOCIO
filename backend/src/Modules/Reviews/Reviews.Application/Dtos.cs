namespace JewelryStore.Modules.Reviews.Application;

public sealed record ReviewDto(
    Guid Id,
    Guid ProductId,
    string AuthorName,
    string Comment,
    int Rating,
    string Status,
    DateTimeOffset CreatedAt);

public sealed record ReviewSummaryDto(
    Guid ProductId,
    double AverageRating,
    int Total,
    IReadOnlyList<ReviewDto> Reviews);

