using AquaLink.Cooperative.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Cooperative.Application.Interfaces;

public interface ICooperativeDbContext
{
    DbSet<CooperativeGroup> CooperativeGroups { get; }
    DbSet<Member> Members { get; }
    DbSet<Contribution> Contributions { get; }
    DbSet<WithdrawalRequest> WithdrawalRequests { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}