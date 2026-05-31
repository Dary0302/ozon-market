namespace StorageService.Domain;

public record Storage : BaseDomainEntity
{
    public required string Address { get; set; }

    public Guid PointId { get; set; }

    public static Storage Restore(Guid id, string address, Guid pointId)
    {
        return new Storage {
            Id = id,
            Address = address,
            PointId = pointId
        };
    }
};