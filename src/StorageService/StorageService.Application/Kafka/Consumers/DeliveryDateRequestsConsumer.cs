using Core.Common.Kafka;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Interfaces;
using Microsoft.Extensions.Options;
using StorageService.Application.Interfaces.Services;
using ProductQuantity = StorageService.Domain.ProductQuantity;

namespace StorageService.Application.Kafka.Consumers;

public class DeliveryDateRequestConsumer(
    IOptions<KafkaSettings> settings,
    IKafkaProducer<KafkaResponse<GetDeliveryDatePayload>> producer,
    IStoredProductService storedProductService)
    : KafkaConsumerService<GetDeliveryDateRequest>(settings, KafkaTopics.DeliveryDateRequests, "storage-service")
{
    protected override async Task HandleAsync(GetDeliveryDateRequest message, CancellationToken token)
    {
        var products = 
            message.Products
            .Select(product => new ProductQuantity(product.ProductId, product.Quantity))
            .ToList();
        
        var result = await storedProductService.GetDeliveryDate(message.PvzId, products, token);

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