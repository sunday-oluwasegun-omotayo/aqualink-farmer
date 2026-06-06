using MediatR;

namespace AquaLink.Cooperative.Application.Cooperatives;

public record RecordContributionCommand(
    Guid CooperativeGroupId,
    Guid MemberId,
    decimal AmountNaira,
    string CycleMonth
) : IRequest<Guid>;