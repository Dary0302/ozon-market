namespace OzonMarketStorage.Domain;

public record StoredProduct(Guid ProductId, Guid StorageId, int Quantity);