using JewelryStore.BuildingBlocks.Domain;
using JewelryStore.Modules.Catalog.Domain;
using Xunit;

namespace JewelryStore.Domain.Tests;

public sealed class ProductTests
{
    [Fact]
    public void Create_NormalizesPriceAndCreatesActiveProduct()
    {
        var product = Product.Create(
            "Cadena Luna",
            "Cadena artesanal",
            Guid.NewGuid(),
            12.345m,
            3,
            null,
            DateTimeOffset.UtcNow);

        Assert.True(product.IsActive);
        Assert.Equal(12.35m, product.Price.Amount);
        Assert.Equal(3, product.Stock);
        Assert.Contains("cadena-luna", product.Slug);
    }

    [Fact]
    public void ChangeStock_RejectsNegativeQuantity()
    {
        var product = Product.Create(
            "Pulsera Sol",
            string.Empty,
            Guid.NewGuid(),
            8m,
            1,
            null,
            DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => product.ChangeStock(-1, DateTimeOffset.UtcNow));
    }
}
