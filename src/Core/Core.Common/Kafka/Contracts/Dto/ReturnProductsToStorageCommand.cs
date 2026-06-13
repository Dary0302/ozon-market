using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record ReturnProductsToStorageCommand(IEnumerable<ProductQuantity> Products, 
    CancellationToken CancellationToken);