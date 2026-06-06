using MediatR;

namespace AquaLink.Prices.Application.Prices;

public record SubmitPriceCommand(
    string Market,
    string Commodity,
    decimal PriceNairaPerKg,
    DateOnly PriceDate,
    Guid SubmittedByAgentId
) : IRequest<Guid>;