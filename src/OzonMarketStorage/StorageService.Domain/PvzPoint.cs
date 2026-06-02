namespace StorageService.Domain;

public record PvzPoint : BaseDomainEntity
{
    public Guid PvzId { get; set; }
    
    public double Longitude { get; set; }
    
    public double Latitude { get; set; }

    public static PvzPoint Restore(Guid id, Guid pvzId, double longitude, double latitude)
    {
        return new PvzPoint
        {
            Id = id,
            PvzId = pvzId,
            Longitude = longitude,
            Latitude = latitude
        };
    }
};