using Core.Common.Kafka;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StorageService.Application.Interfaces.Services;
using ProductQuantity = StorageService.Domain.ProductQuantity;

namespace StorageService.Application.Kafka.Consumers;

public class DeliveryDateRequestConsumer
    : KafkaConsumerService<GetDeliveryDateRequest>
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IKafkaProducer producer;
    
    public DeliveryDateRequestConsumer(
        IOptions<KafkaSettings> settings,
        IServiceScopeFactory scopeFactory,
        IKafkaProducer producer)
        : base(settings, KafkaTopics.DeliveryDateRequests, "storage-service")
    {
        this.scopeFactory = scopeFactory;
        this.producer = producer;
    }
    
    
    protected override async Task HandleAsync(GetDeliveryDateRequest message, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();

        var service =
            scope.ServiceProvider
                .GetRequiredService<IStoredProductService>();
        
        var products = 
            message.Products
            .Select(product => new ProductQuantity(product.ProductId, product.Quantity))
            .ToList();
        
        var result = await service.GetDeliveryDate(message.PvzId, products, token);

        var payload = new GetDeliveryDatePayload {
            IsAvailable = result.IsSuccess, 
            Date = result.Value
        };
        
        var response = result.IsSuccess
            ? KafkaResponse<GetDeliveryDatePayload>.Success(message.CorrelationId, payload)
            : KafkaResponse<GetDeliveryDatePayload>.Failure(message.CorrelationId, "DELIVERY_DATE_ERROR", result.Errors.First().Message);

        await producer.ProduceAsync(KafkaTopics.DeliveryDateResponses, response);
    }
}