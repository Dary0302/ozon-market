namespace StorageService.Domain;

public record StoredProduct(Guid ProductId, Guid StorageId, int Quantity);