using AquaLink.Cooperative.Application.Interfaces;
using AquaLink.Cooperative.Domain.Entities;
using MediatR;

namespace AquaLink.Cooperative.Application.Cooperatives;

public class CreateCooperativeGroupHandler
    : IRequestHandler<CreateCooperativeGroupCommand, Guid>
{
    private readonly ICooperativeDbContext _context;

    public CreateCooperativeGroupHandler(ICooperativeDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateCooperativeGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = CooperativeGroup.Create(
            request.Name,
            request.Description,
            request.TreasurerUserId,
            request.TreasurerFullName,
            request.TreasurerPhoneNumber);

        _context.CooperativeGroups.Add(group);
        await _context.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}