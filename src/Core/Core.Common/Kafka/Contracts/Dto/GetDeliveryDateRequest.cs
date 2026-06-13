using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record GetDeliveryDateRequest(Guid CorrelationId, 
    Guid PvzId, 
    IEnumerable<ProductQuantity> Products, 
    CancellationToken CancellationToken) : IHasCorrelationId;