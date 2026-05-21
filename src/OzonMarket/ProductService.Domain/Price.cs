namespace ProductService.Domain;

public record Price(DateTime Data, double Cost, int Discount) : BaseEntity;