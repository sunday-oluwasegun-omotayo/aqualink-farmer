using AquaLink.Farmer.Application.Interfaces;
using AquaLink.Farmer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Farmer.Infrastructure.Persistence;

public class AquaLinkFarmerDbContext : DbContext, IFarmerDbContext
{
    public AquaLinkFarmerDbContext(
        DbContextOptions<AquaLinkFarmerDbContext> options) : base(options) { }

    public DbSet<FarmCycle> FarmCycles => Set<FarmCycle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FarmCycle>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Species)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PondSizeSqm)
                .HasPrecision(10, 2);

            entity.Property(e => e.Status)
                .HasConversion<string>();
        });
    }
}