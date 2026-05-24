namespace OzonMarketStorage.Domain;

public abstract record BaseDomainEntity
{
    public Guid Id { get; set; }
};