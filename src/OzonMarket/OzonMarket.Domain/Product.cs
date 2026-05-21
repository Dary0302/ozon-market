namespace OzonMarket.Domain;

public record Product(string Name, string Description, double Price, ProductType Type) : BaseEntity;