using AquaLink.Prices.Application.Interfaces;
using AquaLink.Prices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Prices.Infrastructure.Persistence;

public class AquaLinkPricesDbContext : DbContext, IPricesDbContext
{
    public AquaLinkPricesDbContext(
        DbContextOptions<AquaLinkPricesDbContext> options)
        : base(options) { }

    public DbSet<PriceIndex> PriceIndexes => Set<PriceIndex>();
    public DbSet<FarmerAlert> FarmerAlerts => Set<FarmerAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceIndex>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Market)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Commodity)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PriceNairaPerKg)
                .HasPrecision(18, 2);

            entity.Property(e => e.ConfidenceScore)
                .HasPrecision(4, 3);

            entity.Property(e => e.Source)
                .HasConversion<string>();

            // Index for fast current price lookups
            entity.HasIndex(e => new { e.Market, e.Commodity, e.PriceDate });
        });

        modelBuilder.Entity<FarmerAlert>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.MessageSent)
                .IsRequired()
                .HasMaxLength(160); // SMS character limit

            entity.Property(e => e.Status)
                .HasConversion<string>();

            // Prevent duplicate alerts per farmer per day
            entity.HasIndex(e => new { e.FarmerId, e.AlertDate })
                .IsUnique();
        });
    }
}