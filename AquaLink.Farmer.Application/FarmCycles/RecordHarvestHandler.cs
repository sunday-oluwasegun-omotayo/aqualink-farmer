using AquaLink.Farmer.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Farmer.Application.FarmCycles;

public class RecordHarvestHandler : IRequestHandler<RecordHarvestCommand>
{
    private readonly IFarmerDbContext _context;

    public RecordHarvestHandler(IFarmerDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        RecordHarvestCommand request,
        CancellationToken cancellationToken)
    {
        var cycle = await _context.FarmCycles
            .FirstOrDefaultAsync(f => f.Id == request.FarmCycleId, cancellationToken);

        if (cycle is null)
            throw new KeyNotFoundException(
                $"Farm cycle {request.FarmCycleId} not found.");

        cycle.RecordHarvest(
            request.HarvestedWeightKg,
            request.SalePricePerKg,
            request.HarvestedAt);

        await _context.SaveChangesAsync(cancellationToken);
    }
}