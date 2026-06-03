using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Api.Dto;

public record GetOrderInfoResponseDto : BaseDto
{
    public required Guid PvzId { get; init; }
    public required string Status { get; init; }
    public required DateTime DeliveryDate { get; init; }
    public required decimal Amount { get; init; }
    public required IEnumerable<OrderItem> Products { get; init; }
}