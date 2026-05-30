namespace StorageService.Domain;

public record StoredProduct
{
    public Guid ProductId { get; set; }
    public Guid StorageId { get; set; }
    public int Quantity { get; set; }
};