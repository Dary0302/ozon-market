namespace OzonMarketStorage.Domain;

public record StoredProduct(Guid Id, Guid StorageId, int Quantity) : BaseDomainEntity(Id);