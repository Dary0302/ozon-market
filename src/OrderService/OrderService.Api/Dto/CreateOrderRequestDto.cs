using Core.Common.Kafka.Contracts.Models;
using OrderService.Application.Models;

namespace OrderService.Api.Dto;

public record CreateOrderRequestDto(
    Guid PvzId, 
    decimal ClientAmount, 
    IEnumerable<ProductQuantity> Products
    );