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

public class CheckStockRequestConsumer(
    IOptions<KafkaSettings> settings,
    IKafkaProducer<KafkaResponse<CheckStockPayload>> producer,
    IStoredProductService storedProductService)
    : KafkaConsumerService<CheckStockRequest>(settings, KafkaTopics.CheckStockRequests, "storage-service")
{
    protected override async Task HandleAsync(CheckStockRequest message, CancellationToken token)
    {
        var products =
            message.Items
                .Select(product => new ProductQuantity(product.ProductId, product.Quantity))
                .ToList();

        var result = await storedProductService.CheckStock(products, token);

        var items = result.Value.Select(item => new StockCheckResult(item.ProductId, item.Difference));

        var payload = new CheckStockPayload
        {
            IsAvailable = result.IsSuccess,
            Items = items
        };

        var response = result.IsSuccess
            ? KafkaResponse<CheckStockPayload>.Success(message.CorrelationId, payload)
            : KafkaResponse<CheckStockPayload>.Failure(message.CorrelationId, "CHECK_STOCK_ERROR", result.Errors.First().Message);

        await producer.ProduceAsync(KafkaTopics.CheckStockResponses, response);
    }
}