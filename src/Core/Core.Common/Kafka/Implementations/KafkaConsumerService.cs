using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Core.Common.Kafka.Implementations;

public abstract class KafkaConsumerService<T> : BackgroundService
{
    private readonly IConsumer<string, string> consumer;
    private readonly string topic;

    protected KafkaConsumerService(IOptions<KafkaSettings> settings, string topic, string groupId)
    {
        this.topic = topic;
        var config = new ConsumerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers,
            GroupId = groupId,
            AutoOffsetReset  = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = consumer.Consume(stoppingToken);
            var message = JsonSerializer.Deserialize<T>(result.Message.Value)!;
            await HandleAsync(message, stoppingToken);
        }
    }

    protected abstract Task HandleAsync(T message, CancellationToken token);
    
    public override void Dispose()
    {
        consumer.Close();
        consumer.Dispose();
        base.Dispose();
    }
}