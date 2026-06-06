namespace StorageService.Domain;

public record StoragePoint : BaseDomainEntity
{
    public Guid StorageId { get; set; }
    
    public double Longitude { get; set; }
    
    public double Latitude { get; set; }

    public static StoragePoint Restore(Guid id, Guid storageId, double longitude, double latitude)
    {
        return new StoragePoint
        {
            Id = id,
            StorageId = storageId,
            Longitude = longitude,
            Latitude = latitude
        };
    }
};