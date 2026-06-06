using AquaLink.Cooperative.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Cooperative.Application.Cooperatives;

public class ApproveWithdrawalHandler
    : IRequestHandler<ApproveWithdrawalCommand>
{
    private readonly ICooperativeDbContext _context;

    public ApproveWithdrawalHandler(ICooperativeDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        ApproveWithdrawalCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _context.CooperativeGroups
            .Include(g => g.Members)
            .Include(g => g.WithdrawalRequests)
            .FirstOrDefaultAsync(g => g.Id == request.CooperativeGroupId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Cooperative group {request.CooperativeGroupId} not found.");

        group.ApproveWithdrawal(
            request.WithdrawalRequestId,
            request.ApprovedByMemberId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}