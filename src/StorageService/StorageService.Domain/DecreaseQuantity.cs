namespace StorageService.Domain;

public record DecreaseQuantity(Guid ProductId, Guid StorageId, int Quantity);