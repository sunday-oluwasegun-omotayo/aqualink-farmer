using MediatR;

namespace AquaLink.Prices.Application.Prices;

public record GetCurrentPricesQuery(string? Market = null)
    : IRequest<List<CurrentPriceDto>>;

public record CurrentPriceDto(
    string Market,
    string Commodity,
    decimal PriceNairaPerKg,
    DateOnly PriceDate,
    decimal ConfidenceScore
);