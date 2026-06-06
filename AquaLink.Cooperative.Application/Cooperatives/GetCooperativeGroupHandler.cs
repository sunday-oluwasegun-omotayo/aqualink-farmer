using AquaLink.Cooperative.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Cooperative.Application.Cooperatives;

public class GetCooperativeGroupHandler
    : IRequestHandler<GetCooperativeGroupQuery, CooperativeGroupDto>
{
    private readonly ICooperativeDbContext _context;

    public GetCooperativeGroupHandler(ICooperativeDbContext context)
    {
        _context = context;
    }

    public async Task<CooperativeGroupDto> Handle(
        GetCooperativeGroupQuery request,
        CancellationToken cancellationToken)
    {
        var group = await _context.CooperativeGroups
            .AsNoTracking()
            .Include(g => g.Members)
            .Include(g => g.Contributions)
            .Include(g => g.WithdrawalRequests)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Cooperative group {request.Id} not found.");

        return new CooperativeGroupDto(
            group.Id,
            group.Name,
            group.Description,
            group.TreasurerMemberId,
            group.Status.ToString(),
            group.TotalBalance,
            group.Members.Count,
            group.Contributions.Count,
            group.WithdrawalRequests.Count(w =>
                w.Status == Domain.Entities.WithdrawalStatus.Pending),
            group.CreatedAt,
            group.Members.Select(m => new MemberDto(
                m.Id,
                m.UserId,
                m.FullName,
                m.PhoneNumber,
                m.Status.ToString(),
                m.JoinedAt)).ToList(),
            group.Contributions
                .OrderByDescending(c => c.RecordedAt)
                .Take(10)
                .Select(c => new ContributionDto(
                    c.Id,
                    c.MemberId,
                    c.AmountNaira,
                    c.CycleMonth,
                    c.Status.ToString(),
                    c.RecordedAt)).ToList()
        );
    }
}