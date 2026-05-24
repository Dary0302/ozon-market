namespace OzonMarketStorage.Domain;

public record StoragePoint(Guid Id, Guid StorageId, double Longitude, double Latitude) : BaseDomainEntity(Id);