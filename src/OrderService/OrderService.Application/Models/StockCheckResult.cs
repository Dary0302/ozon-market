namespace OrderService.Application.Models;

public record StockCheckResult(Guid ProductId, int Difference);