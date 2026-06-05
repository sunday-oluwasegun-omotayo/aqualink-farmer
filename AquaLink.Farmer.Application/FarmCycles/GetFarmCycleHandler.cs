using AquaLink.Farmer.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Farmer.Application.FarmCycles;

public class GetFarmCycleHandler : IRequestHandler<GetFarmCycleQuery, FarmCycleDto>
{
    private readonly IFarmerDbContext _context;

    public GetFarmCycleHandler(IFarmerDbContext context)
    {
        _context = context;
    }

    public async Task<FarmCycleDto> Handle(
        GetFarmCycleQuery request,
        CancellationToken cancellationToken)
    {
        var cycle = await _context.FarmCycles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (cycle is null)
            throw new KeyNotFoundException($"Farm cycle {request.Id} not found.");

        return new FarmCycleDto(
            cycle.Id,
            cycle.FarmerId,
            cycle.Species,
            cycle.StockedQuantity,
            cycle.PondSizeSqm,
            cycle.StockedAt,
            cycle.ExpectedHarvestAt,
            cycle.Status.ToString(),
            cycle.CreatedAt
        );
    }
}