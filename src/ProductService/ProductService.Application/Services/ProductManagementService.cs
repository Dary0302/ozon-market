using Core.Common.Errors;
using FluentResults;
using ProductService.Application.Dto;
using ProductService.Application.Interfaces;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services;

public class ProductManagementService(IProductRepository productRepository, IPhotoService photoService)
    : IProductManagementService
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
    
    public async Task<Result<IReadOnlyCollection<Product?>>> GetProducts(ProductFilter filter)
    {
        var products = await productRepository.GetProductsByFilter(filter);

        if (products.Count == 0)
        {
            return Result.Fail(AppError.NotFound("Нет продуктов по заданному фильтру"));
        }
        
        return Result.Ok(products);
    }

    public async Task<Result<Guid>> AddProduct(CreateProductDto productDto)
    {
        var addPhotoResult =
            await photoService.AddPhotoAsync(new AddPhotoDto { PhotoData = productDto.PhotoData }, new());
        if (addPhotoResult.IsFailed)
        {
            return Result.Fail(AppError.UnprocessableContent());
        }

        var product = new Product(productDto.Name, productDto.Description, productDto.Type, addPhotoResult.Value);
        
        await productRepository.Add(product);

        return Result.Ok(product.Id);
    }

    public async Task<Result> UpdateProduct(Guid id, CreateProductDto productDto)
    {
        var existingProduct = await productRepository.Get(id);
        if (existingProduct is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        var addPhotoResult =
            await photoService.AddPhotoAsync(new AddPhotoDto { PhotoData = productDto.PhotoData }, new());
        if (addPhotoResult.IsFailed)
        {
            return Result.Fail(AppError.UnprocessableContent());
        }
        
        var product = new Product(productDto.Name, productDto.Description, productDto.Type, addPhotoResult.Value);
        await productRepository.Update(id, product);

        var deletePhotoResult = await photoService.DeletePhotoByIdAsync(existingProduct.PhotoId, new());
        if (deletePhotoResult.IsFailed)
        {
            return Result.Ok()
                .WithError("Товар успешно обновлён, старое фото для удаления не найдено");
        }
        
        return Result.Ok();
    }

    public async Task<Result> DeleteProduct(Guid id)
    {
        var existingProduct = await productRepository.Get(id);
        if (existingProduct is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        await productRepository.Delete(id);

        var deletePhotoResult = await photoService.DeletePhotoByIdAsync(existingProduct.PhotoId, new());
        if (deletePhotoResult.IsFailed)
        {
            return Result.Ok()
                .WithError("Товар успешно удалён, но фото для удаления не найдено");
        }

        return Result.Ok();
    }
}