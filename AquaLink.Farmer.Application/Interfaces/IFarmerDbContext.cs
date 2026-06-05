using AquaLink.Farmer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Farmer.Application.Interfaces;

public interface IFarmerDbContext
{
    DbSet<FarmCycle> FarmCycles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}