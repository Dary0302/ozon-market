using OrderService.Domain;

namespace OrderService.Api.Dto;

public record OrderResponseDto : BaseDto
{
    public required Guid PvzId { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime DeliveryDate { get; init; }
    public required decimal Amount { get; init; }
}