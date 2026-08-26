using System.Threading.RateLimiting;
using JewelryStore.Api;
using JewelryStore.Api.Endpoints;
using JewelryStore.Api.Security;
using JewelryStore.Api.Storage;
using JewelryStore.BuildingBlocks.Application;
using JewelryStore.Modules.Catalog.Infrastructure;
using JewelryStore.Modules.Reviews.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Supabase")
    ?? throw new InvalidOperationException("ConnectionStrings:Supabase is required.");

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddScoped<MessageDispatcher>();
builder.Services.AddScoped<IImageStorage, ProductImageStorage>();
builder.Services.AddCatalogModule(connectionString);
builder.Services.AddReviewsModule(connectionString);

builder.Services
    .AddAuthentication(SupabaseAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, SupabaseAuthenticationHandler>(
        SupabaseAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("admin", policy => policy.RequireRole("admin")));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("reviews", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 4,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0
            }));
});

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
    .ToArray() ?? [];
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    }));

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
}
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapGet("/api/health", async (
    CatalogDbContext catalog,
    ReviewsDbContext reviews,
    CancellationToken cancellationToken) =>
{
    var databaseAvailable =
        await catalog.Database.CanConnectAsync(cancellationToken) &&
        await reviews.Database.CanConnectAsync(cancellationToken);
    return databaseAvailable
        ? Results.Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow })
        : Results.Problem(statusCode: 503, title: "Database unavailable");
}).WithTags("Sistema");

app.MapCatalogEndpoints();
app.MapReviewEndpoints();
app.MapAdminEndpoints();

await DatabaseInitializer.Initialize(app);
await app.RunAsync();

public partial class Program;
