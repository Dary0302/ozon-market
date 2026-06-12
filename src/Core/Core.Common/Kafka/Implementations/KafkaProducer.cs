using Core.Common.Kafka.Interfaces;
using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Core.Common.Kafka.Implementations;

public class KafkaProducer<T> : IKafkaProducer<T>, IDisposable
{
    private readonly IProducer<string, string> producer;

    public KafkaProducer(IOptions<KafkaSettings> settings)
    {
        var config = new ProducerConfig { BootstrapServers = settings.Value.BootstrapServers };
        producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, T message, string? key = null)
    {
        var json = JsonSerializer.Serialize(message);
        await producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = key ?? Guid.NewGuid().ToString(),
            Value = json
        });
    }

    public void Dispose()
    {
        producer.Flush(TimeSpan.FromSeconds(5));
        producer.Dispose();
    }
}