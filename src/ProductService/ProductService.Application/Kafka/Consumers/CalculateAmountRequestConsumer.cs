using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Interfaces;
using Microsoft.Extensions.Options;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Kafka.Consumers;

public class CalculateAmountRequestConsumer
    : KafkaConsumerService<CalculateAmountRequest>
{
    private readonly IPriceService service;
    private readonly IKafkaProducer producer;
    
    public CalculateAmountRequestConsumer(
        IOptions<KafkaSettings> settings,
        IPriceService service,
        IKafkaProducer producer)
        : base(settings, KafkaTopics.GetProductsPriceResponses, "product-service")
    {
        this.service = service;
        this.producer = producer;
    }

    protected override async Task HandleAsync(
        CalculateAmountRequest request,
        CancellationToken ct)
    {
        var result =
            await service.CalculateAmount(
                request.Items,
                ct);

        var response =
            KafkaResponse<CalculateAmountPayload>
                .Success(
                    request.CorrelationId,
                    new CalculateAmountPayload{ Amount = result.Value });

        await producer.ProduceAsync(
            KafkaTopics.CalculateAmountResponses,
            response);
    }
}