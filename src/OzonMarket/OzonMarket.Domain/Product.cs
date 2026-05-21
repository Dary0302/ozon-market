namespace OzonMarket.Domain;

public record Product(string Name, string Description, ProductType Type) : BaseEntity;