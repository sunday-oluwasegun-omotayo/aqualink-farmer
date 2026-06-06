using AquaLink.Prices.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace AquaLink.Prices.Infrastructure.Services;

public class TermiiSmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<TermiiSmsService> _logger;

    public TermiiSmsService(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<TermiiSmsService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<bool> SendAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = phoneNumber,
                from = _config["Termii:SenderId"] ?? "AquaLink",
                sms = message,
                type = "plain",
                channel = "generic",
                api_key = _config["Termii:ApiKey"]
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(
                json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://v3.api.termii.com/api/sms/send",
                content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "SMS sent successfully to {PhoneNumber}", phoneNumber);
                return true;
            }

            _logger.LogWarning(
                "SMS failed for {PhoneNumber}. Status: {Status}",
                phoneNumber, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Exception sending SMS to {PhoneNumber}", phoneNumber);
            return false;
        }
    }
}