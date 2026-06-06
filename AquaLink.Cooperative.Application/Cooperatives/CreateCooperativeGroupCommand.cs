using MediatR;

namespace AquaLink.Cooperative.Application.Cooperatives;

public record CreateCooperativeGroupCommand(
    string Name,
    string Description,
    Guid TreasurerUserId,
    string TreasurerFullName,
    string TreasurerPhoneNumber
) : IRequest<Guid>;