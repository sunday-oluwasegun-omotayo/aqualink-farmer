using AquaLink.Cooperative.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Cooperative.Application.Cooperatives;

public class RequestWithdrawalHandler
    : IRequestHandler<RequestWithdrawalCommand, Guid>
{
    private readonly ICooperativeDbContext _context;

    public RequestWithdrawalHandler(ICooperativeDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        RequestWithdrawalCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _context.CooperativeGroups
            .Include(g => g.Members)
            .Include(g => g.Contributions)
            .Include(g => g.WithdrawalRequests)
            .FirstOrDefaultAsync(g => g.Id == request.CooperativeGroupId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Cooperative group {request.CooperativeGroupId} not found.");

        var withdrawal = group.RequestWithdrawal(
            request.RequestedByMemberId,
            request.AmountNaira,
            request.Reason);

        _context.WithdrawalRequests.Add(withdrawal);
        await _context.SaveChangesAsync(cancellationToken);

        return withdrawal.Id;
    }
}