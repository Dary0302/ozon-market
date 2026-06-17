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

public class ProductStorageRequestConsumer(
    IOptions<KafkaSettings> settings,
    IKafkaProducer producer,
    IStoredProductService storedProductService)
    : KafkaConsumerService<GetOrderStorageRecordsRequest>(settings, KafkaTopics.OrderStorageRecordsRequests, "storage-service")
{
    protected override async Task HandleAsync(GetOrderStorageRecordsRequest message, CancellationToken token)
    {
        var products =
            message.Products
                .Select(product => new ProductQuantity(product.ProductId, product.Quantity))
                .ToList();

        var result = await storedProductService.GetOrderStoragesRecords(message.PvzId, products, token);

        var items = result.Value.Select(item => new DecreaseQuantity(item.ProductId, item.StorageId, item.Quantity));

        var payload = new GetOrderStorageRecordsPayload
        {
            IsAvailable = result.IsSuccess, 
            Items = items
        };

        var response = result.IsSuccess
            ? KafkaResponse<GetOrderStorageRecordsPayload>.Success(message.CorrelationId, payload)
            : KafkaResponse<GetOrderStorageRecordsPayload>.Failure(message.CorrelationId, "PRODUCT_STORAGE_ERROR", result.Errors.First().Message);

        await producer.ProduceAsync(KafkaTopics.OrderStorageRecordsResponses, response);
    }
}