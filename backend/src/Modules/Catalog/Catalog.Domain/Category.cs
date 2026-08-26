using JewelryStore.BuildingBlocks.Domain;

namespace JewelryStore.Modules.Catalog.Domain;

public sealed class Category : AggregateRoot
{
    private Category() { }

    private Category(Guid id, string name, string slug, DateTimeOffset now)
    {
        Id = id;
        Name = name;
        Slug = slug;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;

    public static Category Create(string name, DateTimeOffset now)
    {
        var normalized = GuardName(name);
        return new Category(Guid.NewGuid(), normalized, Slugifier.Create(normalized), now);
    }

    public void Rename(string name, DateTimeOffset now)
    {
        Name = GuardName(name);
        Slug = Slugifier.Create(Name);
        Touch(now);
    }

    private static string GuardName(string name)
    {
        var value = name?.Trim() ?? string.Empty;
        if (value.Length is < 2 or > 80)
        {
            throw new DomainException("La categoría debe tener entre 2 y 80 caracteres.");
        }

        return value;
    }
}

