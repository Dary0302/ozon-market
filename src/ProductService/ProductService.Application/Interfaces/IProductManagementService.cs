using FluentResults;
using ProductService.Application.Dto;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IProductManagementService
{
    Task<Result<Product>> GetProduct(Guid id, CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<Product?>>> GetProducts(ProductFilter filter, CancellationToken cancellationToken);

    Task<Result<Guid>> AddProduct(CreateProductDto productDto, CancellationToken cancellationToken);

    Task<Result> UpdateProduct(Guid id, CreateProductDto productDto, CancellationToken cancellationToken);
    
    Task<Result> DeleteProduct(Guid id, CancellationToken cancellationToken);
}