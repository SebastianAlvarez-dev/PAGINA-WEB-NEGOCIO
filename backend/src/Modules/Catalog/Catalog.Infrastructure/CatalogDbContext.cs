using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Catalog.Application;
using JewelryStore.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace JewelryStore.Modules.Catalog.Infrastructure;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : DbContext(options),
        IProductRepository,
        ICategoryRepository,
        ICatalogUnitOfWork,
        ICatalogReader,
        IProductExistence
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(90).IsRequired();
            entity.HasIndex(category => category.Slug).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).HasMaxLength(140).IsRequired();
            entity.Property(product => product.Slug).HasMaxLength(160).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(2_000).IsRequired();
            entity.Property(product => product.ImageUrl).HasMaxLength(1_000);
            entity.HasIndex(product => product.Slug).IsUnique();
            entity.HasIndex(product => new { product.CategoryId, product.IsActive });
            entity.OwnsOne(product => product.Price, money =>
            {
                money.Property(value => value.Amount)
                    .HasColumnName("price_amount")
                    .HasPrecision(12, 2)
                    .IsRequired();
                money.Property(value => value.Currency)
                    .HasColumnName("price_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    Task IProductRepository.Add(Product product, CancellationToken cancellationToken)
        => Products.AddAsync(product, cancellationToken).AsTask();

    Task<Product?> IProductRepository.Get(Guid id, CancellationToken cancellationToken)
        => Products.SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

    Task ICategoryRepository.Add(Category category, CancellationToken cancellationToken)
        => Categories.AddAsync(category, cancellationToken).AsTask();

    Task<Category?> ICategoryRepository.Get(Guid id, CancellationToken cancellationToken)
        => Categories.SingleOrDefaultAsync(category => category.Id == id, cancellationToken);

    Task<bool> ICategoryRepository.SlugExists(
        string slug,
        Guid? exceptId,
        CancellationToken cancellationToken)
        => Categories.AnyAsync(
            category => category.Slug == slug && (exceptId == null || category.Id != exceptId),
            cancellationToken);

    Task<int> ICatalogUnitOfWork.SaveChanges(CancellationToken cancellationToken)
        => SaveChangesAsync(cancellationToken);

    async Task<IReadOnlyList<CategoryDto>> ICatalogReader.Categories(
        CancellationToken cancellationToken)
        => await Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDto(category.Id, category.Name, category.Slug))
            .ToListAsync(cancellationToken);

    async Task<PagedProducts> ICatalogReader.Products(
        ProductFilter filter,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var query =
            from product in Products.AsNoTracking()
            join category in Categories.AsNoTracking() on product.CategoryId equals category.Id
            where filter.IncludeInactive || product.IsActive
            select new { Product = product, Category = category };

        if (filter.CategoryId is not null)
        {
            query = query.Where(item => item.Product.CategoryId == filter.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = $"%{filter.Search.Trim()}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.Product.Name, pattern) ||
                EF.Functions.ILike(item.Product.Description, pattern));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(item => item.Product.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new ProductCardDto(
                item.Product.Id,
                item.Product.Name,
                item.Product.Slug,
                item.Product.Description,
                item.Product.CategoryId,
                item.Category.Name,
                item.Product.Price.Amount,
                item.Product.Price.Currency,
                item.Product.Stock,
                item.Product.ImageUrl,
                item.Product.IsActive))
            .ToListAsync(cancellationToken);

        return new PagedProducts(items, total, page, pageSize);
    }

    Task<ProductDetailsDto?> ICatalogReader.ProductBySlug(
        string slug,
        CancellationToken cancellationToken)
        => ProductDetails(product => product.Slug == slug && product.IsActive, cancellationToken);

    Task<ProductDetailsDto?> ICatalogReader.ProductById(
        Guid id,
        CancellationToken cancellationToken)
        => ProductDetails(product => product.Id == id, cancellationToken);

    Task<bool> IProductExistence.Exists(Guid productId, CancellationToken cancellationToken)
        => Products.AnyAsync(product => product.Id == productId && product.IsActive, cancellationToken);

    private Task<ProductDetailsDto?> ProductDetails(
        System.Linq.Expressions.Expression<Func<Product, bool>> predicate,
        CancellationToken cancellationToken)
        => (
            from product in Products.AsNoTracking().Where(predicate)
            join category in Categories.AsNoTracking() on product.CategoryId equals category.Id
            select new ProductDetailsDto(
                product.Id,
                product.Name,
                product.Slug,
                product.Description,
                product.CategoryId,
                category.Name,
                product.Price.Amount,
                product.Price.Currency,
                product.Stock,
                product.ImageUrl,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt))
            .SingleOrDefaultAsync(cancellationToken);
}

