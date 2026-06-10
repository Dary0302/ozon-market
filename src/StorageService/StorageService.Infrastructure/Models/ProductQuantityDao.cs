namespace StorageService.Infrastructure.Models;

public record ProductQuantityDao
{
    public Guid ProductId { get; init; }
    
    public int Quantity { get; init; }
};