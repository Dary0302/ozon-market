namespace OzonMarketStorage.Domain;

public record PvzPoint(Guid Id, Guid PvzId, double Longitude, double Latitude) : BaseDomainEntity(Id);