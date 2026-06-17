using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Kafka.Consumers;

public class CalculateAmountRequestConsumer
    : KafkaConsumerService<CalculateAmountRequest>
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IKafkaProducer producer;
    
    public CalculateAmountRequestConsumer(
        IOptions<KafkaSettings> settings,
        IServiceScopeFactory scopeFactory,
        IKafkaProducer producer)
        : base(settings, KafkaTopics.CalculateAmountRequests, "product-service")
    {
        this.scopeFactory = scopeFactory;
        this.producer = producer;
    }

    protected override async Task HandleAsync(
        CalculateAmountRequest request,
        CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var service =
            scope.ServiceProvider
                .GetRequiredService<IPriceService>();

        var result =
            await service.CalculateAmount(
                request.Items,
                ct);

        var response =
            KafkaResponse<CalculateAmountPayload>
                .Success(
                    request.CorrelationId,
                    new CalculateAmountPayload
                    {
                        Amount = result.Value
                    });

        await producer.ProduceAsync(
            KafkaTopics.CalculateAmountResponses,
            response);
    }
}