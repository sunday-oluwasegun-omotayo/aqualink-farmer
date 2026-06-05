using AquaLink.Farmer.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Farmer.Application.FarmCycles;

public class GetFarmCyclesQueryHandler
    : IRequestHandler<GetFarmCyclesQuery, List<FarmCycleDto>>
{
    private readonly IFarmerDbContext _context;

    public GetFarmCyclesQueryHandler(IFarmerDbContext context)
    {
        _context = context;
    }

    public async Task<List<FarmCycleDto>> Handle(
        GetFarmCyclesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.FarmCycles
            .AsNoTracking()
            .Where(f => f.FarmerId == request.FarmerId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FarmCycleDto(
                f.Id,
                f.FarmerId,
                f.Species,
                f.StockedQuantity,
                f.PondSizeSqm,
                f.StockedAt,
                f.ExpectedHarvestAt,
                f.Status.ToString(),
                f.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}