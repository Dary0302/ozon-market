using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Api.Dto;

public record GetOrderInfoResponseDto( 
    Guid PvzId, 
    Status Status,
    DateTime DeliveryDate,
    decimal Amount,
    IEnumerable<OrderItem> Products) : BaseDto;