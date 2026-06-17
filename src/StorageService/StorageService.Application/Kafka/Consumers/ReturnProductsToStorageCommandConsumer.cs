using Core.Common.Kafka;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Implementations;
using Microsoft.Extensions.Options;
using StorageService.Application.Interfaces.Services;
using ProductQuantity = StorageService.Domain.ProductQuantity;

namespace StorageService.Application.Kafka.Consumers;

public class ReturnProductsToStorageCommandConsumer(
    IOptions<KafkaSettings> settings,
    IStoredProductService storedProductService)
    : KafkaConsumerService<ReturnProductsToStorageCommand>(settings, KafkaTopics.ReturnProductsCommand, "storage-service")
{
    protected override async Task HandleAsync(ReturnProductsToStorageCommand message, CancellationToken token)
    {
        var products =
            message.Products
                .Select(product => new ProductQuantity(product.ProductId, product.Quantity))
                .ToList();

        await storedProductService.ReturnProducts(products, token);
    }
}