namespace OrderService.Application.Models;

public record DecreaseQuantity(Guid ProductId, Guid StorageId, int Quantity);