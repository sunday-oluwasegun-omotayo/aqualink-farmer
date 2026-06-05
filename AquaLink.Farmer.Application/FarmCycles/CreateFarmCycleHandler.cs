using AquaLink.Farmer.Application.Interfaces;
using AquaLink.Farmer.Domain.Entities;
using MediatR;

namespace AquaLink.Farmer.Application.FarmCycles;

public class CreateFarmCycleHandler : IRequestHandler<CreateFarmCycleCommand, Guid>
{
    private readonly IFarmerDbContext _context;

    public CreateFarmCycleHandler(IFarmerDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateFarmCycleCommand request,
        CancellationToken cancellationToken)
    {
        var farmCycle = FarmCycle.Create(
            request.FarmerId,
            request.Species,
            request.StockedQuantity,
            request.PondSizeSqm,
            request.StockedAt,
            request.ExpectedHarvestAt
        );

        _context.FarmCycles.Add(farmCycle);
        await _context.SaveChangesAsync(cancellationToken);

        return farmCycle.Id;
    }
}