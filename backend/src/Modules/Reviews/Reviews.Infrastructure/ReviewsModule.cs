using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Reviews.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JewelryStore.Modules.Reviews.Infrastructure;

public static class ReviewsModule
{
    public static IServiceCollection AddReviewsModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ReviewsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations", "reviews")));

        services.AddScoped<IReviewRepository>(Provider);
        services.AddScoped<IReviewUnitOfWork>(Provider);
        services.AddScoped<IReviewReader>(Provider);

        services.AddScoped<ICommandHandler<SubmitReviewCommand, Guid>, SubmitReviewHandler>();
        services.AddScoped<ICommandHandler<ModerateReviewCommand, bool>, ModerateReviewHandler>();
        services.AddScoped<IQueryHandler<GetProductReviewsQuery, ReviewSummaryDto>, GetProductReviewsHandler>();
        services.AddScoped<IQueryHandler<GetPendingReviewsQuery, IReadOnlyList<ReviewDto>>, GetPendingReviewsHandler>();
        return services;
    }

    private static ReviewsDbContext Provider(IServiceProvider services)
        => services.GetRequiredService<ReviewsDbContext>();
}
