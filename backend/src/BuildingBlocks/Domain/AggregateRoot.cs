namespace JewelryStore.BuildingBlocks.Domain;

public abstract class AggregateRoot
{
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset UpdatedAt { get; protected set; }

    protected void Touch(DateTimeOffset now) => UpdatedAt = now;
}

