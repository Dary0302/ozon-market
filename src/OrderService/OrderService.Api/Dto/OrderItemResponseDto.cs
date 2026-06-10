namespace OrderService.Api.Dto;

public record OrderItemResponseDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
};