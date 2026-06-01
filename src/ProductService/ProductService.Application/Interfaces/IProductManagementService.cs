using FluentResults;
using ProductService.Application.Dto;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IProductManagementService
{
    Task<Result<Product>> GetProduct(Guid id);

    Task<Result<Guid>> AddProduct(CreateProductDto productDto);

    Task<Result> UpdateProduct(Guid id, CreateProductDto productDto);
    
    Task<Result> DeleteProduct(Guid id);
}