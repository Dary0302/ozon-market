namespace StorageService.Application.Dto;

public record AddStoredProductDto
{
    public Guid ProductId { get; init; }
    
    public Guid StorageId { get; init; }
    
    public int Quantity { get; set; }
};