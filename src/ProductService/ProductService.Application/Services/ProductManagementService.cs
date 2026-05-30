using Core.Common.Errors;
using FluentResults;
using ProductService.Application.Interfaces;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services;

public class ProductManagementService(IProductRepository productRepository) : IProductManagementService
{
    private const string NotFoundExceptionMessage = "Продукт не найден";
    
    public async Task<Result<Product>> GetProduct(Guid id)
    {
        var product = await productRepository.Get(id);

        if (product is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }

        return Result.Ok(product);
    }

    public async Task<Result<Guid>> AddProduct(Product product)
    {
        await productRepository.Add(product);

        return Result.Ok(product.Id);
    }

    public async Task<Result<bool>> UpdateProduct(Guid id, Product product)
    {
        var existingProduct = await productRepository.Get(id);

        if (existingProduct is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }

        await productRepository.Update(id, product);

        return Result.Ok();
    }

    public async Task<Result<bool>> DeleteProduct(Guid id)
    {
        var existingProduct = await productRepository.Get(id);

        if (existingProduct is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }

        await productRepository.Delete(id);

        return Result.Ok();
    }
}