namespace ProductService.Domain;

public record Price(DateTime Date, double Cost, decimal Discount) : BaseEntity;