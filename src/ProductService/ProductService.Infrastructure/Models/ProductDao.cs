using ProductService.Domain;

namespace ProductService.Infrastructure.Models;

public record ProductDao : BaseEntityDao
{
    public string Name { get; init; }
    public string Description { get; init; }
    public ProductType Type { get; init; }
    public Guid? PhotoId { get; init; }
}