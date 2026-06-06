namespace AquaLink.Cooperative.Domain.Entities;

public class WithdrawalRequest
{
    public Guid Id { get; private set; }
    public Guid CooperativeGroupId { get; private set; }
    public Guid RequestedByMemberId { get; private set; }
    public Guid? ApprovedByMemberId { get; private set; }
    public decimal AmountNaira { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public WithdrawalStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }

    private WithdrawalRequest() { }

    internal static WithdrawalRequest Create(
        Guid cooperativeGroupId,
        Guid requestedByMemberId,
        decimal amountNaira,
        string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new WithdrawalRequest
        {
            Id = Guid.NewGuid(),
            CooperativeGroupId = cooperativeGroupId,
            RequestedByMemberId = requestedByMemberId,
            AmountNaira = amountNaira,
            Reason = reason.Trim(),
            Status = WithdrawalStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
    }

    internal void Approve(Guid approvedByMemberId)
    {
        if (Status != WithdrawalStatus.Pending)
            throw new InvalidOperationException(
                "Only pending withdrawals can be approved.");

        ApprovedByMemberId = approvedByMemberId;
        Status = WithdrawalStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
    }
}

public enum WithdrawalStatus { Pending, Approved, Rejected }