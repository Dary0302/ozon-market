using FluentResults;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IProductManagementService
{
    Task<Result<Product>> GetProduct(Guid id);
    
    Task<Result<Guid>> AddProduct(Product product);
    
    Task<Result> UpdateProduct(Guid id, Product product);
    
    Task<Result> DeleteProduct(Guid id);
}