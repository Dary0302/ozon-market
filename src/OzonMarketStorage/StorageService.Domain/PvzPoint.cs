namespace StorageService.Domain;

public record PvzPoint(Guid PvzId, double Longitude, double Latitude) : BaseDomainEntity;