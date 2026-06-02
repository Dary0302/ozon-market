namespace StorageService.Domain;

public record IncreaseQuantity(Guid ProductId, Guid StorageId, int Quantity);