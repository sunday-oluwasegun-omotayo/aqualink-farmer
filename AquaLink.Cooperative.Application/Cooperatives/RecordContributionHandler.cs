using AquaLink.Cooperative.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Cooperative.Application.Cooperatives;

public class RecordContributionHandler
    : IRequestHandler<RecordContributionCommand, Guid>
{
    private readonly ICooperativeDbContext _context;

    public RecordContributionHandler(ICooperativeDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        RecordContributionCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _context.CooperativeGroups
            .Include(g => g.Members)
            .Include(g => g.Contributions)
            .FirstOrDefaultAsync(g => g.Id == request.CooperativeGroupId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Cooperative group {request.CooperativeGroupId} not found.");

        var contribution = group.RecordContribution(
            request.MemberId,
            request.AmountNaira,
            request.CycleMonth);

        // Explicitly track the new contribution
        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync(cancellationToken);

        return contribution.Id;
    }
}