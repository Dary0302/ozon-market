namespace ProductService.Infrastructure.Models;

public record PriceDao(DateTime Data, double Cost, int Discount) : BaseEntityDao;