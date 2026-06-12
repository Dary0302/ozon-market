using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Contracts.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Application.Consumers;

public class GetProductsPriceResponseConsumer  
    : KafkaConsumerService<KafkaResponse<GetProductsPricePayload>>
{
    private readonly PendingRequestRegistry registry;

    public GetProductsPriceResponseConsumer(
        IOptions<KafkaSettings> settings, 
        PendingRequestRegistry registry)
        : base(settings, KafkaTopics.CalculateAmountResponses, "order-service") 
    {
        this.registry = registry;
    }

    protected override Task HandleAsync(KafkaResponse<GetProductsPricePayload> message, CancellationToken token)
    {
        registry.Complete(message.CorrelationId, message);
        return Task.CompletedTask;
    }
}