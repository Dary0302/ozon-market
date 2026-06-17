using Core.Common.Kafka;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Implementations;
using Microsoft.Extensions.Options;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Kafka.Consumers;

public class ReduceCountOfProductsCommandConsumer(
    IOptions<KafkaSettings> settings,
    IStoredProductService storedProductService)
    : KafkaConsumerService<ReduceCountOfProductsCommand>(settings, KafkaTopics.ReduceStockCommand, "storage-service")
{
    protected override async Task HandleAsync(ReduceCountOfProductsCommand message, CancellationToken token)
    {
        var products =
            message.Items
                .Select(product => new DecreaseQuantity(product.ProductId, product.StorageId, product.Quantity))
                .ToList();

        await storedProductService.DecreaseStoredProductQuantity(products, token);
    }
}