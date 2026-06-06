using AquaLink.Prices.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AquaLink.Prices.Application.Prices;

public class GetCurrentPricesHandler
    : IRequestHandler<GetCurrentPricesQuery, List<CurrentPriceDto>>
{
    private readonly IPricesDbContext _context;

    public GetCurrentPricesHandler(IPricesDbContext context)
    {
        _context = context;
    }

    public async Task<List<CurrentPriceDto>> Handle(
        GetCurrentPricesQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var query = _context.PriceIndexes
            .AsNoTracking()
            .Where(p => p.PriceDate == today);

        if (!string.IsNullOrWhiteSpace(request.Market))
            query = query.Where(p => p.Market == request.Market);

        return await query
            .OrderByDescending(p => p.ConfidenceScore)
            .Select(p => new CurrentPriceDto(
                p.Market,
                p.Commodity,
                p.PriceNairaPerKg,
                p.PriceDate,
                p.ConfidenceScore))
            .ToListAsync(cancellationToken);
    }
}