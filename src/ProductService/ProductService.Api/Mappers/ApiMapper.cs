using ProductService.Application.Dto;
using ProductService.Domain;

namespace ProductService.Api.Mappers;

public static class ApiMapper
{
    public static UpdateProductDto? ToHttp(this Product? product)
    {
        if (product is null)
        {
            return null;
        }
        
        return new UpdateProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Type = product.Type,
            PhotoId = product.PhotoId
        };
    }
}