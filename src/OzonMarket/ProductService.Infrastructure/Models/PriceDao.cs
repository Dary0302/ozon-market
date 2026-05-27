namespace ProductService.Infrastructure.Models;

public record PriceDao(DateTime Date, double Cost, int Discount) : BaseEntityDao;