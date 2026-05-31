namespace StorageService.Domain;

public record Storage(string Address, Guid PointId) : BaseDomainEntity;