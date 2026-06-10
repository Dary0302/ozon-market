namespace OrderService.Application.Models;

public record ProductPrice(Guid ProductId, decimal Price, DateTime Date);