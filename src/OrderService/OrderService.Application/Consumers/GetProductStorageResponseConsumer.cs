using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Contracts.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Application.Consumers;

public class GetProductStorageResponseConsumer  
    : KafkaConsumerService<KafkaResponse<GetProductStoragePayload>>
{
    private readonly PendingRequestRegistry registry;

    public GetProductStorageResponseConsumer(
        IOptions<KafkaSettings> settings, 
        PendingRequestRegistry registry)
        : base(settings, KafkaTopics.ProductStorageResponses, "order-service") 
    {
        this.registry = registry;
    }

    protected override Task HandleAsync(KafkaResponse<GetProductStoragePayload> message, CancellationToken token)
    {
        registry.Complete(message.CorrelationId, message);
        return Task.CompletedTask;
    }
}