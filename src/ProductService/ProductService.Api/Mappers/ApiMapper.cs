using ProductService.Application.Dto;
using ProductService.Domain;

namespace ProductService.Api.Mappers;

public static class ApiMapper
{
    public static GetProductDto? ToHttp(this Product? product)
    {
        if (product is null)
        {
            return null;
        }
        
        return new GetProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Type = product.Type.ToHttp(),
            PhotoId = product.PhotoId
        };
    }

    public static string ToHttp(this ProductType type)
    {
        return type switch
        {
            ProductType.Undefined => "Undefined",
            ProductType.Phone => "Телефон",
            ProductType.Tablet => "Планшет",
            ProductType.Headphones => "Наушники",
            ProductType.Laptop => "Ноутбук",
            ProductType.TV => "ТВ",
            ProductType.Smartwatch => "Умные часы",
            ProductType.Console => "Консоль",
            ProductType.Camera => "Камера",
            ProductType.Monitor => "Монитор",
            var _ => type.ToString()
        };
    }
}