using Core.Common.Kafka.Contracts.Models;

namespace OrderService.Application.Kafka.Interfaces;

public interface IStockMutationService
{
    Task ReduceStock(IEnumerable<DecreaseQuantity> productStock);
    Task ReturnStock(IEnumerable<ProductQuantity> products);
}