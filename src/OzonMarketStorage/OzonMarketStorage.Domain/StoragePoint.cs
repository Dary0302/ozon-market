namespace OzonMarketStorage.Domain;

public record StoragePoint(Guid PointId, Guid StorageId, double Longitude, double Latitude);