using Microsoft.AspNetCore.Http;
using ProductService.Domain;

namespace ProductService.Application.Dto;

public record CreateProductDto
{
    public string Name { get; init; }
    public string Description { get; init; }
    public ProductType Type { get; init; }
    public IFormFile? PhotoData { get; init; }
}