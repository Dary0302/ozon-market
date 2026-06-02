namespace ProductService.Infrastructure.Models;

public record PriceDao(Guid ProductId, DateTime Date, double Cost, int Discount) : BaseEntityDao;