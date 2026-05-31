namespace StorageService.Domain;

public record StockCheckResult
{
    public Guid ProductId { get; set; }
    
    public int Difference { get; set; }
};