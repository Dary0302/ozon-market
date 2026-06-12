using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record CalculateAmountRequest(Guid CorrelationId, IEnumerable<ProductQuantity> Items) : IHasCorrelationId;