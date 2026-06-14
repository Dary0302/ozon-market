using Core.Common.Kafka.Contracts.Models;
using FluentResults;

namespace Core.Common.Kafka.Contracts.Services;

public interface IStorageService
{
    Task<IEnumerable<StockCheckResult>> CheckStock(IEnumerable<ProductQuantity> items, CancellationToken ct);
    Task<DateTime> GetDeliveryDate(Guid pvzId, IEnumerable<ProductQuantity> products, CancellationToken ct);
    Task ReduceCountOfProducts(IEnumerable<DecreaseQuantity> items, CancellationToken ct);
    Task<IEnumerable<DecreaseQuantity>> GetOrderStorageRecords(Guid pvzId, IEnumerable<ProductQuantity> products, CancellationToken ct);
    Task ReturnProductsToStorage(IEnumerable<ProductQuantity> products, CancellationToken ct);
}