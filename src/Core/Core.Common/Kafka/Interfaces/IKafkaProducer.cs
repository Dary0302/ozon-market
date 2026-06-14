namespace Core.Common.Kafka.Interfaces;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(
        string topic, 
        T message, 
        CancellationToken ct = default, 
        string? key = null);
}