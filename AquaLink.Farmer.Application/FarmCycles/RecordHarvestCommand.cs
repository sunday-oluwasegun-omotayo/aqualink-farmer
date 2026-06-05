using MediatR;

namespace AquaLink.Farmer.Application.FarmCycles;

public record RecordHarvestCommand(
    Guid FarmCycleId,
    decimal HarvestedWeightKg,
    decimal SalePricePerKg,
    DateOnly HarvestedAt
) : IRequest;