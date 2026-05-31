namespace StorageService.Infrastructure.Models;

public record StoredProductDao(Guid ProductId, Guid StorageId, int Quantity);