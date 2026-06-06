using AquaLink.Cooperative.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Cooperative.Application.Cooperatives;

public class AddMemberHandler : IRequestHandler<AddMemberCommand, Guid>
{
    private readonly ICooperativeDbContext _context;

    public AddMemberHandler(ICooperativeDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        AddMemberCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _context.CooperativeGroups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(
                g => g.Id == request.CooperativeGroupId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Cooperative group {request.CooperativeGroupId} not found.");

        var member = group.AddMember(
            request.UserId,
            request.FullName,
            request.PhoneNumber);

        // Explicitly add the new member to the Members DbSet
        // so EF Core tracks it as a new INSERT, not an update
        _context.Members.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        return member.Id;
    }
}