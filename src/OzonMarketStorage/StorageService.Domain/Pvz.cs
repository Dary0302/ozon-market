namespace StorageService.Domain;

public record Pvz : BaseDomainEntity
{
    public required string Address { get; set; }
    public Guid PointId { get; set; }
};