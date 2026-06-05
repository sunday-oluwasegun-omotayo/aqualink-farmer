using MediatR;

namespace AquaLink.Farmer.Application.FarmCycles;

public record GetFarmCyclesQuery(Guid FarmerId) : IRequest<List<FarmCycleDto>>;