namespace AquaLink.Farmer.Domain.Entities;

public class FarmCycle
{
    public Guid Id { get; private set; }
    public Guid FarmerId { get; private set; }
    public string Species { get; private set; } = string.Empty;
    public int StockedQuantity { get; private set; }
    public decimal PondSizeSqm { get; private set; }
    public DateOnly StockedAt { get; private set; }
    public DateOnly? ExpectedHarvestAt { get; private set; }
    public FarmCycleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private FarmCycle() { }

    public static FarmCycle Create(
        Guid farmerId,
        string species,
        int stockedQuantity,
        decimal pondSizeSqm,
        DateOnly stockedAt,
        DateOnly? expectedHarvestAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(species);

        if (stockedQuantity <= 0)
            throw new ArgumentException("Stocked quantity must be positive.");

        if (pondSizeSqm <= 0)
            throw new ArgumentException("Pond size must be positive.");

        return new FarmCycle
        {
            Id = Guid.NewGuid(),
            FarmerId = farmerId,
            Species = species.Trim(),
            StockedQuantity = stockedQuantity,
            PondSizeSqm = pondSizeSqm,
            StockedAt = stockedAt,
            ExpectedHarvestAt = expectedHarvestAt,
            Status = FarmCycleStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public enum FarmCycleStatus
{
    Active,
    Harvested,
    Lost
}