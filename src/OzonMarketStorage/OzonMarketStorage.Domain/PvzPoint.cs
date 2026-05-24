namespace OzonMarketStorage.Domain;

public record PvzPoint : BaseDomainEntity
{
    public Guid PvzId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
};