using AquaLink.Farmer.Domain.Entities;
using FluentAssertions;

namespace AquaLink.Farmer.Tests;

public class FarmCycleTests
{
    private static FarmCycle CreateValidCycle() => FarmCycle.Create(
        farmerId: Guid.NewGuid(),
        species: "Catfish",
        stockedQuantity: 500,
        pondSizeSqm: 200,
        stockedAt: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
        expectedHarvestAt: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))
    );

    // ── Create ──────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidInputs_ShouldReturnActiveCycle()
    {
        var cycle = CreateValidCycle();

        cycle.Should().NotBeNull();
        cycle.Id.Should().NotBeEmpty();
        cycle.Species.Should().Be("Catfish");
        cycle.StockedQuantity.Should().Be(500);
        cycle.PondSizeSqm.Should().Be(200);
        cycle.Status.Should().Be(FarmCycleStatus.Active);
        cycle.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_ShouldTrimSpeciesWhitespace()
    {
        var cycle = FarmCycle.Create(
            Guid.NewGuid(), "  Tilapia  ", 100, 50,
            DateOnly.FromDateTime(DateTime.UtcNow));

        cycle.Species.Should().Be("Tilapia");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptySpecies_ShouldThrow(string? species)
    {
        var act = () => FarmCycle.Create(
            Guid.NewGuid(), species!, 500, 200,
            DateOnly.FromDateTime(DateTime.UtcNow));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithZeroOrNegativeQuantity_ShouldThrow(int quantity)
    {
        var act = () => FarmCycle.Create(
            Guid.NewGuid(), "Catfish", quantity, 200,
            DateOnly.FromDateTime(DateTime.UtcNow));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*positive*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithZeroOrNegativePondSize_ShouldThrow(decimal pondSize)
    {
        var act = () => FarmCycle.Create(
            Guid.NewGuid(), "Catfish", 500, pondSize,
            DateOnly.FromDateTime(DateTime.UtcNow));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*positive*");
    }

    // ── RecordHarvest ───────────────────────────────────────────────────

    [Fact]
    public void RecordHarvest_OnActiveCycle_ShouldSetStatusToHarvested()
    {
        var cycle = CreateValidCycle();

        cycle.RecordHarvest(180.5m, 1800m,
            DateOnly.FromDateTime(DateTime.UtcNow));

        cycle.Status.Should().Be(FarmCycleStatus.Harvested);
        cycle.HarvestedWeightKg.Should().Be(180.5m);
        cycle.SalePricePerKg.Should().Be(1800m);
    }

    [Fact]
    public void RecordHarvest_OnHarvestedCycle_ShouldThrowInvalidOperation()
    {
        var cycle = CreateValidCycle();
        cycle.RecordHarvest(180m, 1800m,
            DateOnly.FromDateTime(DateTime.UtcNow));

        var act = () => cycle.RecordHarvest(200m, 1900m,
            DateOnly.FromDateTime(DateTime.UtcNow));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*active*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RecordHarvest_WithZeroOrNegativeWeight_ShouldThrow(
        decimal weight)
    {
        var cycle = CreateValidCycle();

        var act = () => cycle.RecordHarvest(weight, 1800m,
            DateOnly.FromDateTime(DateTime.UtcNow));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*positive*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-500)]
    public void RecordHarvest_WithZeroOrNegativePrice_ShouldThrow(
        decimal price)
    {
        var cycle = CreateValidCycle();

        var act = () => cycle.RecordHarvest(180m, price,
            DateOnly.FromDateTime(DateTime.UtcNow));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*positive*");
    }
}