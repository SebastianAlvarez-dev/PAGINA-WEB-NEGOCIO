using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JewelryStore.BuildingBlocks.Application;
using JewelryStore.BuildingBlocks.Domain;

namespace JewelryStore.Api.Storage;

public sealed class ProductImageStorage(
    IConfiguration configuration,
    IWebHostEnvironment environment,
    IHttpClientFactory httpClientFactory,
    ILogger<ProductImageStorage> logger) : IImageStorage
{
    private static readonly HashSet<string> AllowedContentTypes =
        ["image/jpeg", "image/png", "image/webp"];

    public async Task<string> UploadProductImage(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (!AllowedContentTypes.Contains(contentType))
        {
            throw new DomainException("Solo se permiten imágenes JPG, PNG o WebP.");
        }

        var extension = contentType switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
        var objectName = $"{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";

        var url = configuration["Supabase:Url"]?.TrimEnd('/');
        var secretKey = configuration["Supabase:SecretKey"];
        var bucket = configuration["Supabase:ProductImagesBucket"] ?? "product-images";

        if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(secretKey))
        {
            await EnsureBucket(url, secretKey, bucket, cancellationToken);
            return await UploadToSupabase(
                url,
                secretKey,
                bucket,
                objectName,
                content,
                contentType,
                cancellationToken);
        }

        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException("Supabase Storage no está configurado.");
        }

        var root = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads");
        var fullPath = Path.Combine(root, objectName.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var destination = File.Create(fullPath);
        await content.CopyToAsync(destination, cancellationToken);
        return $"/uploads/{objectName}";
    }

    private async Task EnsureBucket(
        string url,
        string secretKey,
        string bucket,
        CancellationToken cancellationToken)
    {
        using var lookup = Authorized(
            HttpMethod.Get,
            $"{url}/storage/v1/bucket/{bucket}",
            secretKey);
        using var lookupResponse = await httpClientFactory.CreateClient().SendAsync(lookup, cancellationToken);
        if (lookupResponse.IsSuccessStatusCode)
        {
            return;
        }

        if (lookupResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException("No se pudo consultar el almacenamiento de imágenes.");
        }

        using var request = Authorized(
            HttpMethod.Post,
            $"{url}/storage/v1/bucket",
            secretKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(new { id = bucket, name = bucket, @public = true }),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClientFactory.CreateClient().SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var detail = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Could not ensure Supabase bucket. Status {Status}: {Detail}", response.StatusCode, detail);
            throw new InvalidOperationException("No se pudo preparar el almacenamiento de imágenes.");
        }
    }

    private async Task<string> UploadToSupabase(
        string url,
        string secretKey,
        string bucket,
        string objectName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken)
    {
        using var request = Authorized(
            HttpMethod.Post,
            $"{url}/storage/v1/object/{bucket}/{objectName}",
            secretKey);
        request.Headers.TryAddWithoutValidation("x-upsert", "false");
        request.Content = new StreamContent(content);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        using var response = await httpClientFactory.CreateClient().SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var detail = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Supabase image upload failed. Status {Status}: {Detail}", response.StatusCode, detail);
            throw new InvalidOperationException("No se pudo subir la imagen.");
        }

        return $"{url}/storage/v1/object/public/{bucket}/{objectName}";
    }

    private static HttpRequestMessage Authorized(HttpMethod method, string url, string secretKey)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.TryAddWithoutValidation("apikey", secretKey);
        return request;
    }
}
