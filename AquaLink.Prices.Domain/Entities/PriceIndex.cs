namespace AquaLink.Prices.Domain.Entities;

public class PriceIndex
{
    public Guid Id { get; private set; }
    public string Market { get; private set; } = string.Empty;
    public string Commodity { get; private set; } = string.Empty;
    public decimal PriceNairaPerKg { get; private set; }
    public DateOnly PriceDate { get; private set; }
    public PriceSource Source { get; private set; }
    public decimal ConfidenceScore { get; private set; }
    public Guid SubmittedByAgentId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PriceIndex() { }

    public static PriceIndex Create(
        string market,
        string commodity,
        decimal priceNairaPerKg,
        DateOnly priceDate,
        PriceSource source,
        Guid submittedByAgentId,
        decimal confidenceScore = 1.0m)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(market);
        ArgumentException.ThrowIfNullOrWhiteSpace(commodity);

        if (priceNairaPerKg <= 0)
            throw new ArgumentException(
                "Price must be greater than zero.");

        if (confidenceScore is < 0 or > 1)
            throw new ArgumentException(
                "Confidence score must be between 0 and 1.");

        return new PriceIndex
        {
            Id = Guid.NewGuid(),
            Market = market.Trim(),
            Commodity = commodity.Trim(),
            PriceNairaPerKg = priceNairaPerKg,
            PriceDate = priceDate,
            Source = source,
            SubmittedByAgentId = submittedByAgentId,
            ConfidenceScore = confidenceScore,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public enum PriceSource
{
    FieldAgent,
    CrowdSourced,
    SystemGenerated
}