namespace ProductService.Domain;

public record Price(DateTime Date, double Cost, int Discount) : BaseEntity;