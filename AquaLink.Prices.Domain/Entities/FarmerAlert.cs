namespace AquaLink.Prices.Domain.Entities;

public class FarmerAlert
{
    public Guid Id { get; private set; }
    public Guid FarmerId { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public string MessageSent { get; private set; } = string.Empty;
    public DateOnly AlertDate { get; private set; }
    public AlertStatus Status { get; private set; }
    public DateTime SentAt { get; private set; }

    private FarmerAlert() { }

    public static FarmerAlert Create(
        Guid farmerId,
        string phoneNumber,
        string messageSent,
        DateOnly alertDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(messageSent);

        return new FarmerAlert
        {
            Id = Guid.NewGuid(),
            FarmerId = farmerId,
            PhoneNumber = phoneNumber,
            MessageSent = messageSent,
            AlertDate = alertDate,
            Status = AlertStatus.Sent,
            SentAt = DateTime.UtcNow
        };
    }
}

public enum AlertStatus { Sent, Failed, Delivered }