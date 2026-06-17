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

public class GetProductsPriceRequestConsumer
    : KafkaConsumerService<GetProductsPriceRequest>
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IKafkaProducer producer;

    public GetProductsPriceRequestConsumer(
        IOptions<KafkaSettings> settings,
        IServiceScopeFactory scopeFactory,
        IKafkaProducer producer)
        : base(settings, KafkaTopics.GetProductsPriceRequests, "product-service")
    {
        this.scopeFactory = scopeFactory;
        this.producer = producer;
    }

    protected override async Task HandleAsync(
        GetProductsPriceRequest request,
        CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        
        var service =
            scope.ServiceProvider
                .GetRequiredService<IPriceService>();
        
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