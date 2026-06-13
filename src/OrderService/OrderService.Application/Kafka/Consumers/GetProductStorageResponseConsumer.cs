using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Contracts.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Application.Kafka.Consumers;

public class GetProductStorageResponseConsumer  
    : KafkaConsumerService<KafkaResponse<GetOrderStorageRecordsPayload>>
{
    private readonly PendingRequestRegistry registry;

    public GetProductStorageResponseConsumer(
        IOptions<KafkaSettings> settings, 
        PendingRequestRegistry registry)
        : base(settings, KafkaTopics.OrderStorageRecordsResponses, "order-service") 
    {
        this.registry = registry;
    }

    protected override Task HandleAsync(KafkaResponse<GetOrderStorageRecordsPayload> message, CancellationToken token)
    {
        registry.Complete(message.CorrelationId, message);
        return Task.CompletedTask;
    }
}