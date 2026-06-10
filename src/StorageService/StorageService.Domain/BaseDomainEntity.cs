namespace StorageService.Domain;

public abstract record BaseDomainEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
};