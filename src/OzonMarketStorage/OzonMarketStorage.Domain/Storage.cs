namespace OzonMarketStorage.Domain;

public record Storage : BaseDomainEntity
{
    public required string Address { get; set; }
    public Guid PointId { get; set; }
};