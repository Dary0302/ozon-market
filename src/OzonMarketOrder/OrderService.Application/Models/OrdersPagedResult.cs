namespace OrderService.Application.Models;

public record OrdersPagedResult<T>(IEnumerable<T> Items, int TotalCount);