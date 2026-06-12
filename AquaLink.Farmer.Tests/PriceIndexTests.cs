using AquaLink.Prices.Domain.Entities;
using FluentAssertions;
using System.Diagnostics;

namespace AquaLink.Farmer.Tests;

public class PriceIndexTests
{
    [Fact]
    public void Create_WithValidInputs_ShouldReturnPriceIndex()
    {
        var agentId = Guid.NewGuid();

        var price = PriceIndex.Create(
            "Epe Fish Market",
            "Catfish",
            1800m,
            DateOnly.FromDateTime(DateTime.UtcNow),
            PriceSource.FieldAgent,
            agentId);

        price.Should().NotBeNull();
        price.Id.Should().NotBeEmpty();
        price.Market.Should().Be("Epe Fish Market");
        price.Commodity.Should().Be("Catfish");
        price.PriceNairaPerKg.Should().Be(1800m);
        price.Source.Should().Be(PriceSource.FieldAgent);
        price.SubmittedByAgentId.Should().Be(agentId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyMarket_ShouldThrow(string? market)
    {
        var act = () => PriceIndex.Create(
            market!, "Catfish", 1800m,
            DateOnly.FromDateTime(DateTime.UtcNow),
            PriceSource.FieldAgent, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Create_WithZeroOrNegativePrice_ShouldThrow(decimal price)
    {
        var act = () => PriceIndex.Create(
            "Epe Fish Market", "Catfish", price,
            DateOnly.FromDateTime(DateTime.UtcNow),
            PriceSource.FieldAgent, Guid.NewGuid());

        act.Should().Throw<ArgumentException>()
            .WithMessage("*greater than zero*");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(2.0)]
    public void Create_WithInvalidConfidenceScore_ShouldThrow(
        double confidence)
    {
        var act = () => PriceIndex.Create(
            "Epe Fish Market", "Catfish", 1800m,
            DateOnly.FromDateTime(DateTime.UtcNow),
            PriceSource.FieldAgent, Guid.NewGuid(),
            (decimal)confidence);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*between 0 and 1*");
    }

    [Fact]
    public void Create_ShouldTrimMarketAndCommodityWhitespace()
    {
        var price = PriceIndex.Create(
            "  Epe Fish Market  ", "  Catfish  ", 1800m,
            DateOnly.FromDateTime(DateTime.UtcNow),
            PriceSource.FieldAgent, Guid.NewGuid());

        price.Market.Should().Be("Epe Fish Market");
        price.Commodity.Should().Be("Catfish");
    }
}