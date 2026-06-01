namespace ProductService.Domain;

public record Product(string Name, string Description, ProductType Type, Guid PhotoId) : BaseEntity;