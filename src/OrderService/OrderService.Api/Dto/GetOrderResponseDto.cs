using OrderService.Domain;

namespace OrderService.Api.Dto;

public record GetOrderResponseDto(
    Guid PvzId, 
    Status Status,
    DateTime DeliveryDate,
    DateTime CreatedOn,
    decimal Amount) : BaseDto;