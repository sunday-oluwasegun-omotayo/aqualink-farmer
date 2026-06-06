using AquaLink.Prices.Application.Interfaces;
using AquaLink.Prices.Domain.Entities;
using MediatR;

namespace AquaLink.Prices.Application.Prices;

public class SubmitPriceHandler : IRequestHandler<SubmitPriceCommand, Guid>
{
    private readonly IPricesDbContext _context;

    public SubmitPriceHandler(IPricesDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        SubmitPriceCommand request,
        CancellationToken cancellationToken)
    {
        var price = PriceIndex.Create(
            request.Market,
            request.Commodity,
            request.PriceNairaPerKg,
            request.PriceDate,
            PriceSource.FieldAgent,
            request.SubmittedByAgentId);

        _context.PriceIndexes.Add(price);
        await _context.SaveChangesAsync(cancellationToken);

        return price.Id;
    }
}