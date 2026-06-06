using AquaLink.Prices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Prices.Application.Interfaces;

public interface IPricesDbContext
{
    DbSet<PriceIndex> PriceIndexes { get; }
    DbSet<FarmerAlert> FarmerAlerts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}