namespace AquaLink.Cooperative.Domain.Entities;

public class Member
{
    public Guid Id { get; private set; }
    public Guid CooperativeGroupId { get; private set; }
    public Guid UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public MemberStatus Status { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private Member() { }

    internal static Member Create(
        Guid cooperativeGroupId,
        Guid userId,
        string fullName,
        string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        return new Member
        {
            Id = Guid.NewGuid(),
            CooperativeGroupId = cooperativeGroupId,
            UserId = userId,
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Status = MemberStatus.Active,
            JoinedAt = DateTime.UtcNow
        };
    }
}

public enum MemberStatus { Active, Suspended, Removed }