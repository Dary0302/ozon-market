namespace Core.Common.Kafka.Interfaces;

public interface IHasCorrelationId
{
    Guid CorrelationId { get; }
}