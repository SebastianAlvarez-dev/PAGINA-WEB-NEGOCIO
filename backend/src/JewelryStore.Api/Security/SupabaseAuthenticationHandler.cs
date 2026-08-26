using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JewelryStore.Api.Security;

public sealed class SupabaseAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IHostEnvironment environment)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "SupabaseBearer";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var token = authorization["Bearer ".Length..].Trim();
        if (environment.IsDevelopment() && token == "dev-admin")
        {
            return Success("development-admin", "admin@localhost", "admin");
        }

        var supabaseUrl = configuration["Supabase:Url"]?.TrimEnd('/');
        var publishableKey = configuration["Supabase:PublishableKey"];
        if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(publishableKey))
        {
            return AuthenticateResult.Fail("Supabase Auth no está configurado.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{supabaseUrl}/auth/v1/user");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        request.Headers.TryAddWithoutValidation("apikey", publishableKey);

        using var response = await httpClientFactory.CreateClient().SendAsync(request, Context.RequestAborted);
        if (!response.IsSuccessStatusCode)
        {
            return AuthenticateResult.Fail("La sesión no es válida o ha expirado.");
        }

        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(Context.RequestAborted));
        var root = document.RootElement;
        var id = root.GetProperty("id").GetString() ?? string.Empty;
        var email = root.TryGetProperty("email", out var emailElement)
            ? emailElement.GetString() ?? string.Empty
            : string.Empty;
        var role = "customer";

        if (root.TryGetProperty("app_metadata", out var metadata) &&
            metadata.TryGetProperty("role", out var roleElement))
        {
            role = roleElement.GetString() ?? role;
        }

        return Success(id, email, role);
    }

    private static AuthenticateResult Success(string id, string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName));
    }
}
