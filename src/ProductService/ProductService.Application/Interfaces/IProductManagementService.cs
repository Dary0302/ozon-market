using FluentResults;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IProductManagementService
{
    /*│   ├── AddProduct
│   ├── UpdateProduct
│   ├── DeleteProduct
│   └── GetProduct
*/
    Task<Result<Product>> GetProduct(Guid id);
    
    Task<Result<Guid>> AddProduct(Product product);
    
    Task<Result<bool>> UpdateProduct(Guid id, Product product);
    
    Task<Result<bool>> DeleteProduct(Guid id);
}