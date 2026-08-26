using JewelryStore.Modules.Catalog.Domain;

namespace JewelryStore.Modules.Catalog.Application;

public interface ICatalogUnitOfWork
{
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public interface IProductRepository
{
    Task Add(Product product, CancellationToken cancellationToken);
    Task<Product?> Get(Guid id, CancellationToken cancellationToken);
}

public interface ICategoryRepository
{
    Task Add(Category category, CancellationToken cancellationToken);
    Task<Category?> Get(Guid id, CancellationToken cancellationToken);
    Task<bool> SlugExists(string slug, Guid? exceptId, CancellationToken cancellationToken);
}

public interface ICatalogReader
{
    Task<IReadOnlyList<CategoryDto>> Categories(CancellationToken cancellationToken);
    Task<PagedProducts> Products(ProductFilter filter, CancellationToken cancellationToken);
    Task<ProductDetailsDto?> ProductBySlug(string slug, CancellationToken cancellationToken);
    Task<ProductDetailsDto?> ProductById(Guid id, CancellationToken cancellationToken);
}

