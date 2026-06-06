using AquaLink.Prices.Application.Interfaces;
using AquaLink.Prices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AquaLink.Prices.Infrastructure.Jobs;

public class DailyPriceAlertJob
{
    private readonly IPricesDbContext _pricesContext;
    private readonly ISmsService _smsService;
    private readonly ILogger<DailyPriceAlertJob> _logger;

    public DailyPriceAlertJob(
        IPricesDbContext pricesContext,
        ISmsService smsService,
        ILogger<DailyPriceAlertJob> logger)
    {
        _pricesContext = pricesContext;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        _logger.LogInformation(
            "Starting daily price alert job for {Date}", today);

        // Get today's prices
        var prices = await _pricesContext.PriceIndexes
            .AsNoTracking()
            .Where(p => p.PriceDate == today)
            .OrderByDescending(p => p.ConfidenceScore)
            .ToListAsync();

        if (!prices.Any())
        {
            _logger.LogWarning(
                "No price data available for {Date}. Alerts skipped.", today);
            return;
        }

        // Build the SMS message
        var priceLines = prices
            .GroupBy(p => p.Commodity)
            .Select(g =>
            {
                var best = g.OrderByDescending(p => p.ConfidenceScore).First();
                return $"{best.Commodity}: N{best.PriceNairaPerKg:N0}/kg";
            });

        var message =
            $"AquaLink Price Alert {today:dd/MM/yyyy}\n" +
            string.Join("\n", priceLines) +
            "\nFor more: aqualink.ng";

        // In production this queries the Farmer module for registered phones
        // For now we use a test list — replace with real farmer phone query
        var testFarmerPhones = new[]
        {
            ("test-farmer-id", "08012345678")
        };

        foreach (var (farmerId, phone) in testFarmerPhones)
        {
            // Skip if already sent today
            var alreadySent = await _pricesContext.FarmerAlerts
                .AnyAsync(a =>
                    a.FarmerId == Guid.Parse(farmerId) &&
                    a.AlertDate == today);

            if (alreadySent) continue;

            var success = await _smsService.SendAsync(phone, message);

            var alert = FarmerAlert.Create(
                Guid.Parse(farmerId),
                phone,
                message,
                today);

            _pricesContext.FarmerAlerts.Add(alert);
        }

        await _pricesContext.SaveChangesAsync(default);

        _logger.LogInformation(
            "Daily price alert job completed for {Date}", today);
    }
}