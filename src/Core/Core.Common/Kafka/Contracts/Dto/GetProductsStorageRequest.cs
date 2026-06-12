using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record GetProductsStorageRequest(Guid CorrelationId, IEnumerable<ProductQuantity> Products) : IHasCorrelationId;