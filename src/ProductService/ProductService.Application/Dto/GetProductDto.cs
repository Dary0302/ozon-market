using ProductService.Domain;

namespace ProductService.Application.Dto;

public record GetProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public Guid? PhotoId { get; set; }
}