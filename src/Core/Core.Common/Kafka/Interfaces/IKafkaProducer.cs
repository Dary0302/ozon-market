namespace Core.Common.Kafka.Interfaces;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(
        string topic,
        T message,
        string? key = null);
}