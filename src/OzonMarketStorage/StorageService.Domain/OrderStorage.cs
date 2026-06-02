namespace StorageService.Domain;

public record OrderStorage(Guid ProductId, Guid StorageId, double Distance);