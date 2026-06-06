using MediatR;

namespace AquaLink.Cooperative.Application.Cooperatives;

public record RequestWithdrawalCommand(
    Guid CooperativeGroupId,
    Guid RequestedByMemberId,
    decimal AmountNaira,
    string Reason
) : IRequest<Guid>;