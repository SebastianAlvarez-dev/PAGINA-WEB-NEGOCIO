namespace JewelryStore.Modules.Catalog.Application;

public sealed record CategoryDto(Guid Id, string Name, string Slug);

public sealed record ProductCardDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    Guid CategoryId,
    string Category,
    decimal Price,
    string Currency,
    int Stock,
    string? ImageUrl,
    bool IsActive);

public sealed record ProductDetailsDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    Guid CategoryId,
    string Category,
    decimal Price,
    string Currency,
    int Stock,
    string? ImageUrl,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ProductFilter(
    string? Search = null,
    Guid? CategoryId = null,
    bool IncludeInactive = false,
    int Page = 1,
    int PageSize = 24);

public sealed record PagedProducts(
    IReadOnlyList<ProductCardDto> Items,
    int Total,
    int Page,
    int PageSize);

