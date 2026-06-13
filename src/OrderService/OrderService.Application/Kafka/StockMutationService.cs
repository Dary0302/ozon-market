using Core.Common.Kafka;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;
using OrderService.Application.Kafka.Interfaces;

namespace OrderService.Application.Kafka;

public class StockMutationService(
    IKafkaProducer<ReduceCountOfProductsCommand> reduceProducer,
    IKafkaProducer<ReturnProductsToStorageCommand> returnProducer)
    : IStockMutationService
{
    public Task ReduceStock(IEnumerable<DecreaseQuantity> productStock) =>
        reduceProducer.ProduceAsync(KafkaTopics.ReduceStockCommand, new ReduceCountOfProductsCommand(productStock));

    public Task ReturnStock(IEnumerable<ProductQuantity> products) =>
        returnProducer.ProduceAsync(KafkaTopics.ReturnProductsCommand, new ReturnProductsToStorageCommand(products));
}