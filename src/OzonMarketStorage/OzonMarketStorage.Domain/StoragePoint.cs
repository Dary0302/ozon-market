namespace OzonMarketStorage.Domain;

public record StoragePoint : BaseDomainEntity
{
    public Guid StorageId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
};