namespace StorageService.Domain;

public record Pvz : BaseDomainEntity
{
    public required string Address { get; set; }
    
    public Guid PointId { get; set; }

    public static Pvz Restore(Guid id, string address, Guid pointId)
    {
        return new Pvz {
            Id = id,
            Address = address,
            PointId = pointId
        };
    }
};