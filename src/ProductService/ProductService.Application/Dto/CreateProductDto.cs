using ProductService.Domain;

namespace ProductService.Application.Dto;

public record CreateProductDto
{
    public string Name { get; init; }
    public string Description { get; init; }
    public ProductType Type { get; init; }
    public byte[] PhotoData { get; init; }
}