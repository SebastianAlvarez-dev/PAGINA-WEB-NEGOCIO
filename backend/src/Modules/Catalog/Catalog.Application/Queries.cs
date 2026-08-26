using JewelryStore.BuildingBlocks.Application;

namespace JewelryStore.Modules.Catalog.Application;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>;

public sealed class GetCategoriesHandler(ICatalogReader reader)
    : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public Task<IReadOnlyList<CategoryDto>> Handle(
        GetCategoriesQuery query,
        CancellationToken cancellationToken)
        => reader.Categories(cancellationToken);
}

public sealed record GetProductsQuery(ProductFilter Filter) : IQuery<PagedProducts>;

public sealed class GetProductsHandler(ICatalogReader reader)
    : IQueryHandler<GetProductsQuery, PagedProducts>
{
    public Task<PagedProducts> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        => reader.Products(query.Filter, cancellationToken);
}

public sealed record GetProductBySlugQuery(string Slug) : IQuery<ProductDetailsDto?>;

public sealed class GetProductBySlugHandler(ICatalogReader reader)
    : IQueryHandler<GetProductBySlugQuery, ProductDetailsDto?>
{
    public Task<ProductDetailsDto?> Handle(
        GetProductBySlugQuery query,
        CancellationToken cancellationToken)
        => reader.ProductBySlug(query.Slug, cancellationToken);
}

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductDetailsDto?>;

public sealed class GetProductByIdHandler(ICatalogReader reader)
    : IQueryHandler<GetProductByIdQuery, ProductDetailsDto?>
{
    public Task<ProductDetailsDto?> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
        => reader.ProductById(query.Id, cancellationToken);
}

