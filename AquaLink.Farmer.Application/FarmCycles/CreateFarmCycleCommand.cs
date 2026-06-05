using AquaLink.Farmer.Domain.Entities;
using MediatR;

namespace AquaLink.Farmer.Application.FarmCycles;

public record CreateFarmCycleCommand(
    Guid FarmerId,
    string Species,
    int StockedQuantity,
    decimal PondSizeSqm,
    DateOnly StockedAt,
    DateOnly? ExpectedHarvestAt
) : IRequest<Guid>;