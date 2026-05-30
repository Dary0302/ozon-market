namespace OrderService.Application.Models;

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount);