namespace AquaLink.Prices.Application.Interfaces;

public interface ISmsService
{
    Task<bool> SendAsync(string phoneNumber, string message,
        CancellationToken cancellationToken = default);
}