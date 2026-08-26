using JewelryStore.BuildingBlocks.Domain;

namespace JewelryStore.Modules.Catalog.Domain;

public sealed record Money
{
    private Money() { }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
        {
            throw new DomainException("El precio no puede ser negativo.");
        }

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();
    }

    public decimal Amount { get; private init; }
    public string Currency { get; private init; } = "USD";
}

