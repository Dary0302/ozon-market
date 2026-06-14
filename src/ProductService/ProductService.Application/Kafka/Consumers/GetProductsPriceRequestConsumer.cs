using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Interfaces;
using Microsoft.Extensions.Options;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Kafka.Consumers;

public class GetProductsPriceRequestConsumer
    : KafkaConsumerService<GetProductsPriceRequest>
{
    private readonly IPriceService service;
    private readonly IKafkaProducer producer;

    public GetProductsPriceRequestConsumer(
        IOptions<KafkaSettings> settings,
        IPriceService service,
        IKafkaProducer producer)
        : base(settings, KafkaTopics.GetProductsPriceResponses, "product-service")
    {
        this.service = service;
        this.producer = producer;
    }

    protected override async Task HandleAsync(
        GetProductsPriceRequest request,
        CancellationToken ct)
    {
        var result = await service.GetActualPrices(request.Request.ProductIds.ToList(),
            request.Request.Date,
            ct);

        var response =
            KafkaResponse<GetProductsPricePayload>
                .Success(request.CorrelationId,
                    new GetProductsPricePayload { ProductPrices = result.Value });

        await producer.ProduceAsync(KafkaTopics.GetProductsPriceResponses,
            response);
    }
}