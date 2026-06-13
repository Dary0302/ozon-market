using Core.Common.Kafka.Contracts.Models;

namespace OrderService.Application.Kafka;

public record OrderData(
    decimal Amount,
    DateTime DeliveryDate,
    IEnumerable<StockCheckResult> StockItems,
    IEnumerable<DecreaseQuantity> ProductStorages);