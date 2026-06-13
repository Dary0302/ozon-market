using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Contracts.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Application.Kafka.Consumers;

public class CalculateAmountResponseConsumer  
    : KafkaConsumerService<KafkaResponse<CalculateAmountPayload>>
{
    private readonly PendingRequestRegistry registry;

    public CalculateAmountResponseConsumer(
        IOptions<KafkaSettings> settings, 
        PendingRequestRegistry registry)
        : base(settings, KafkaTopics.CalculateAmountResponses, "order-service") 
    {
        this.registry = registry;
    }

    protected override Task HandleAsync(KafkaResponse<CalculateAmountPayload> message, CancellationToken token)
    {
        registry.Complete(message.CorrelationId, message);
        return Task.CompletedTask;
    }
}