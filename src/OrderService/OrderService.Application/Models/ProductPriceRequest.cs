namespace OrderService.Application.Models;

public record ProductPriceRequest(DateTime Date, IEnumerable<Guid> ProductIds);