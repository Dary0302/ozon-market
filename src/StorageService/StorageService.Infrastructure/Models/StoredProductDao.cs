namespace StorageService.Infrastructure.Models;

public record StoredProductDao
{
    public Guid ProductId { get; init; }
    
    public Guid StorageId { get; init; }
    
    public int Quantity { get; init; }
};