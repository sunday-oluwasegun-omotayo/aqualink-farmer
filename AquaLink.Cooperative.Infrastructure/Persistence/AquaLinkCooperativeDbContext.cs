using AquaLink.Cooperative.Application.Interfaces;
using AquaLink.Cooperative.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Cooperative.Infrastructure.Persistence;

public class AquaLinkCooperativeDbContext : DbContext, ICooperativeDbContext
{
    public AquaLinkCooperativeDbContext(
        DbContextOptions<AquaLinkCooperativeDbContext> options)
        : base(options) { }

    public DbSet<CooperativeGroup> CooperativeGroups => Set<CooperativeGroup>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Contribution> Contributions => Set<Contribution>();
    public DbSet<WithdrawalRequest> WithdrawalRequests => Set<WithdrawalRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CooperativeGroup
        modelBuilder.Entity<CooperativeGroup>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .HasConversion<string>();

            // Owned collections
            entity.HasMany(e => e.Members)
                .WithOne()
                .HasForeignKey(m => m.CooperativeGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Contributions)
                .WithOne()
                .HasForeignKey(c => c.CooperativeGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.WithdrawalRequests)
                .WithOne()
                .HasForeignKey(w => w.CooperativeGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Member
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Status)
                .HasConversion<string>();
        });

        // Contribution — append only, never updated
        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AmountNaira)
                .HasPrecision(18, 2);

            entity.Property(e => e.CycleMonth)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Status)
                .HasConversion<string>();
        });

        // WithdrawalRequest
        modelBuilder.Entity<WithdrawalRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AmountNaira)
                .HasPrecision(18, 2);

            entity.Property(e => e.Reason)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .HasConversion<string>();
        });
    }
}