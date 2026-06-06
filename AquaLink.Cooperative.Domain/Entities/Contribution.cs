namespace AquaLink.Cooperative.Domain.Entities;

public class Contribution
{
    public Guid Id { get; private set; }
    public Guid CooperativeGroupId { get; private set; }
    public Guid MemberId { get; private set; }
    public decimal AmountNaira { get; private set; }
    public string CycleMonth { get; private set; } = string.Empty;
    public ContributionStatus Status { get; private set; }
    public DateTime RecordedAt { get; private set; }

    private Contribution() { }

    internal static Contribution Create(
        Guid cooperativeGroupId,
        Guid memberId,
        decimal amountNaira,
        string cycleMonth)
    {
        return new Contribution
        {
            Id = Guid.NewGuid(),
            CooperativeGroupId = cooperativeGroupId,
            MemberId = memberId,
            AmountNaira = amountNaira,
            CycleMonth = cycleMonth.Trim(),
            Status = ContributionStatus.Confirmed,
            RecordedAt = DateTime.UtcNow
        };
    }
}

public enum ContributionStatus { Confirmed, Reversed }