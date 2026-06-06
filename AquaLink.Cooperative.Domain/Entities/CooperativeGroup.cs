namespace AquaLink.Cooperative.Domain.Entities;

public class CooperativeGroup
{
    private readonly List<Member> _members = new();
    private readonly List<Contribution> _contributions = new();
    private readonly List<WithdrawalRequest> _withdrawalRequests = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid TreasurerMemberId { get; internal set; }
    public CooperativeStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Member> Members => _members.AsReadOnly();
    public IReadOnlyCollection<Contribution> Contributions => _contributions.AsReadOnly();
    public IReadOnlyCollection<WithdrawalRequest> WithdrawalRequests
        => _withdrawalRequests.AsReadOnly();

    public decimal TotalBalance => _contributions
        .Where(c => c.Status == ContributionStatus.Confirmed)
        .Sum(c => c.AmountNaira)
        - _withdrawalRequests
        .Where(w => w.Status == WithdrawalStatus.Approved)
        .Sum(w => w.AmountNaira);

    private CooperativeGroup() { }

    public static CooperativeGroup Create(
    string name,
    string description,
    Guid treasurerUserId,
    string treasurerFullName,
    string treasurerPhoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var group = new CooperativeGroup
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description.Trim(),
            Status = CooperativeStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        // Add treasurer as first member
        var treasurer = group.AddMember(
            treasurerUserId,
            treasurerFullName,
            treasurerPhoneNumber);

        group.TreasurerMemberId = treasurer.Id;
        return group;
    }

    public Member AddMember(
        Guid userId,
        string fullName,
        string phoneNumber)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new InvalidOperationException(
                "This user is already a member of this cooperative.");

        var member = Member.Create(Id, userId, fullName, phoneNumber);
        _members.Add(member);
        return member;
    }

    public Contribution RecordContribution(
        Guid memberId,
        decimal amountNaira,
        string cycleMonth)
    {
        if (!_members.Any(m => m.Id == memberId))
            throw new InvalidOperationException(
                "Only registered members can make contributions.");

        if (amountNaira <= 0)
            throw new ArgumentException(
                "Contribution amount must be greater than zero.");

        var contribution = Contribution.Create(
            Id, memberId, amountNaira, cycleMonth);
        _contributions.Add(contribution);
        return contribution;
    }

    public WithdrawalRequest RequestWithdrawal(
        Guid requestedByMemberId,
        decimal amountNaira,
        string reason)
    {
        if (!_members.Any(m => m.Id == requestedByMemberId))
            throw new InvalidOperationException(
                "Only registered members can request withdrawals.");

        if (amountNaira <= 0)
            throw new ArgumentException(
                "Withdrawal amount must be greater than zero.");

        if (amountNaira > TotalBalance)
            throw new InvalidOperationException(
                $"Insufficient balance. Available: ₦{TotalBalance:N2}");

        var request = WithdrawalRequest.Create(
            Id, requestedByMemberId, amountNaira, reason);
        _withdrawalRequests.Add(request);
        return request;
    }

    public void ApproveWithdrawal(
        Guid withdrawalRequestId,
        Guid approvedByMemberId)
    {
        if (approvedByMemberId != TreasurerMemberId)
            throw new InvalidOperationException(
                "Only the treasurer can approve withdrawals.");

        var request = _withdrawalRequests
            .FirstOrDefault(w => w.Id == withdrawalRequestId)
            ?? throw new KeyNotFoundException(
                "Withdrawal request not found.");

        request.Approve(approvedByMemberId);
    }
}

public enum CooperativeStatus { Active, Suspended, Dissolved }