using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Contracts.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Application.Consumers;

public class CheckStockResponseConsumer 
    : KafkaConsumerService<KafkaResponse<CheckStockPayload>>
{
    private readonly PendingRequestRegistry registry;

    public CheckStockResponseConsumer(
        IOptions<KafkaSettings> settings, 
        PendingRequestRegistry registry)
        : base(settings, KafkaTopics.CheckStockResponses, "order-service") 
    {
        this.registry = registry;
    }

    protected override Task HandleAsync(KafkaResponse<CheckStockPayload> message, CancellationToken token)
    {
        registry.Complete(message.CorrelationId, message);
        return Task.CompletedTask;
    }
}