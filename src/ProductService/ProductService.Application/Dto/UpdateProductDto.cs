using ProductService.Domain;

namespace ProductService.Application.Dto;

public record UpdateProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductType Type { get; set; }
    public Guid PhotoId { get; set; }
}