using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Catalog.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JewelryStore.Modules.Catalog.Infrastructure;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations", "catalog")));

        services.AddScoped<IProductRepository>(Provider);
        services.AddScoped<ICategoryRepository>(Provider);
        services.AddScoped<ICatalogUnitOfWork>(Provider);
        services.AddScoped<ICatalogReader>(Provider);
        services.AddScoped<IProductExistence>(Provider);

        services.AddScoped<ICommandHandler<CreateCategoryCommand, Guid>, CreateCategoryHandler>();
        services.AddScoped<ICommandHandler<CreateProductCommand, Guid>, CreateProductHandler>();
        services.AddScoped<ICommandHandler<UpdateProductCommand, bool>, UpdateProductHandler>();
        services.AddScoped<ICommandHandler<ArchiveProductCommand, bool>, ArchiveProductHandler>();
        services.AddScoped<IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>, GetCategoriesHandler>();
        services.AddScoped<IQueryHandler<GetProductsQuery, PagedProducts>, GetProductsHandler>();
        services.AddScoped<IQueryHandler<GetProductBySlugQuery, ProductDetailsDto?>, GetProductBySlugHandler>();
        services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductDetailsDto?>, GetProductByIdHandler>();
        return services;
    }

    private static CatalogDbContext Provider(IServiceProvider services)
        => services.GetRequiredService<CatalogDbContext>();
}

