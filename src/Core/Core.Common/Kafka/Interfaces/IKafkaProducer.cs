namespace Core.Common.Kafka.Interfaces;

public interface IKafkaProducer<T>
{
    Task ProduceAsync(string topic, T message, string? key = null);
}