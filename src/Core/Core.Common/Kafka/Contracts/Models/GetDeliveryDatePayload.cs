namespace Core.Common.Kafka.Contracts.Models;

public class GetDeliveryDatePayload
{
    public bool IsAvailable { get; init; }
    public DateTime Date { get; init; }
}