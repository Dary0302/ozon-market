using FluentResults;
using ProductService.Application.Interfaces;
using ProductService.Domain;

namespace ProductService.Application.Services;

public class ProductManagementService : IProductManagementService
{
    public Task<Result<Product>> GetProduct(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Guid>> AddProduct(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateProduct(Guid id, Product product)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteProduct(Guid id)
    {
        throw new NotImplementedException();
    }
}