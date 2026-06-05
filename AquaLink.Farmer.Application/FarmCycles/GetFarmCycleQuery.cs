using MediatR;

namespace AquaLink.Farmer.Application.FarmCycles;

public record GetFarmCycleQuery(Guid Id) : IRequest<FarmCycleDto>;

public record FarmCycleDto(
    Guid Id,
    Guid FarmerId,
    string Species,
    int StockedQuantity,
    decimal PondSizeSqm,
    DateOnly StockedAt,
    DateOnly? ExpectedHarvestAt,
    string Status,
    DateTime CreatedAt
);