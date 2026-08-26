using JewelryStore.BuildingBlocks.Domain;

namespace JewelryStore.Modules.Catalog.Domain;

public sealed class Product : AggregateRoot
{
    private Product() { }

    private Product(
        Guid id,
        string name,
        string description,
        Guid categoryId,
        Money price,
        int stock,
        string? imageUrl,
        DateTimeOffset now)
    {
        Id = id;
        Name = name;
        Slug = Slugifier.Create(name, id);
        Description = description;
        CategoryId = categoryId;
        Price = price;
        Stock = stock;
        ImageUrl = imageUrl;
        IsActive = true;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public Money Price { get; private set; } = new(0);
    public int Stock { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    public static Product Create(
        string name,
        string description,
        Guid categoryId,
        decimal price,
        int stock,
        string? imageUrl,
        DateTimeOffset now)
    {
        var id = Guid.NewGuid();
        return new Product(
            id,
            GuardName(name),
            GuardDescription(description),
            categoryId,
            new Money(price),
            GuardStock(stock),
            NormalizeImage(imageUrl),
            now);
    }

    public void Update(
        string name,
        string description,
        Guid categoryId,
        decimal price,
        int stock,
        string? imageUrl,
        bool isActive,
        DateTimeOffset now)
    {
        Name = GuardName(name);
        Slug = Slugifier.Create(Name, Id);
        Description = GuardDescription(description);
        CategoryId = categoryId;
        Price = new Money(price);
        Stock = GuardStock(stock);
        ImageUrl = NormalizeImage(imageUrl);
        IsActive = isActive;
        Touch(now);
    }

    public void ChangeStock(int quantity, DateTimeOffset now)
    {
        Stock = GuardStock(quantity);
        Touch(now);
    }

    public void Archive(DateTimeOffset now)
    {
        IsActive = false;
        Touch(now);
    }

    private static string GuardName(string name)
    {
        var value = name?.Trim() ?? string.Empty;
        if (value.Length is < 2 or > 140)
        {
            throw new DomainException("El nombre debe tener entre 2 y 140 caracteres.");
        }

        return value;
    }

    private static string GuardDescription(string description)
    {
        var value = description?.Trim() ?? string.Empty;
        if (value.Length > 2_000)
        {
            throw new DomainException("La descripción no puede superar los 2000 caracteres.");
        }

        return value;
    }

    private static int GuardStock(int stock)
    {
        if (stock < 0)
        {
            throw new DomainException("El stock no puede ser negativo.");
        }

        return stock;
    }

    private static string? NormalizeImage(string? imageUrl)
        => string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
}

