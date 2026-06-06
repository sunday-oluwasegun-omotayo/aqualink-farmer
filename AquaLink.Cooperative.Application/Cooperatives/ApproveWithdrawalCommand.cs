using MediatR;

namespace AquaLink.Cooperative.Application.Cooperatives;

public record ApproveWithdrawalCommand(
    Guid CooperativeGroupId,
    Guid WithdrawalRequestId,
    Guid ApprovedByMemberId
) : IRequest;