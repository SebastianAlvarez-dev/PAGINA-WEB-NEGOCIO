using JewelryStore.Modules.Catalog.Domain;
using JewelryStore.Modules.Catalog.Infrastructure;
using JewelryStore.Modules.Reviews.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JewelryStore.Api;

public static class DatabaseInitializer
{
    public static async Task Initialize(WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();
        var catalog = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var reviews = scope.ServiceProvider.GetRequiredService<ReviewsDbContext>();

        await catalog.Database.MigrateAsync();
        await reviews.Database.MigrateAsync();

        if (app.Configuration.GetValue<bool>("Database:SeedDemoData") &&
            !await catalog.Categories.AnyAsync())
        {
            var now = DateTimeOffset.UtcNow;
            catalog.Categories.AddRange(
                Category.Create("Cadenas", now),
                Category.Create("Pulseras", now),
                Category.Create("Aretes", now),
                Category.Create("Anillos", now),
                Category.Create("Adornos", now));
            await catalog.SaveChangesAsync();
        }
    }
}

