using MediatR;

namespace AquaLink.Cooperative.Application.Cooperatives;

public record GetCooperativeGroupQuery(Guid Id) : IRequest<CooperativeGroupDto>;

public record CooperativeGroupDto(
    Guid Id,
    string Name,
    string Description,
    Guid TreasurerMemberId,
    string Status,
    decimal TotalBalanceNaira,
    int MemberCount,
    int ContributionCount,
    int PendingWithdrawals,
    DateTime CreatedAt,
    IReadOnlyList<MemberDto> Members,
    IReadOnlyList<ContributionDto> RecentContributions
);

public record MemberDto(
    Guid Id,
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string Status,
    DateTime JoinedAt
);

public record ContributionDto(
    Guid Id,
    Guid MemberId,
    decimal AmountNaira,
    string CycleMonth,
    string Status,
    DateTime RecordedAt
);