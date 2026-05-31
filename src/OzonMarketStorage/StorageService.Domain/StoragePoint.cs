namespace StorageService.Domain;

public record StoragePoint(Guid StorageId, double Longitude, double Latitude) : BaseDomainEntity;