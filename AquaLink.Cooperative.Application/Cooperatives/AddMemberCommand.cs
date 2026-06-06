using MediatR;

namespace AquaLink.Cooperative.Application.Cooperatives;

public record AddMemberCommand(
    Guid CooperativeGroupId,
    Guid UserId,
    string FullName,
    string PhoneNumber
) : IRequest<Guid>;