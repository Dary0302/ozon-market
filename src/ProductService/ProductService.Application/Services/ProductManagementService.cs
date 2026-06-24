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

    public async Task<Result<Product>> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var product = await productRepository.Get(id, cancellationToken);

        if (product is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }

        return Result.Ok(product);
    }
    
    public async Task<Result<IReadOnlyCollection<Product?>>> GetProducts(ProductFilter filter, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsByFilter(filter, cancellationToken);

        if (products.Count == 0)
        {
            return Result.Fail(AppError.NotFound("Нет продуктов по заданному фильтру"));
        }
        
        return Result.Ok(products);
    }

    public async Task<Result<Guid>> AddProduct(CreateProductDto productDto, CancellationToken cancellationToken)
    {
        Guid? photoId = null;
        if (productDto.PhotoData is not null && productDto.PhotoData.Length > 0)
        {
            var addPhotoResult =
                await photoService.AddPhotoAsync(new AddPhotoDto { PhotoData = productDto.PhotoData }, cancellationToken);
            if (addPhotoResult.IsFailed)
            {
                return Result.Fail(AppError.UnprocessableContent());
            }
            
            photoId = addPhotoResult.Value;
        }

        var product = new Product(productDto.Name, productDto.Description, productDto.Type, photoId);
        
        await productRepository.Add(product, cancellationToken);

        return Result.Ok(product.Id);
    }

    public async Task<Result> UpdateProduct(Guid id, CreateProductDto productDto, CancellationToken cancellationToken)
    {
        var existingProduct = await productRepository.Get(id, cancellationToken);
        if (existingProduct is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        Guid? photoId = null;
        if (productDto.PhotoData is not null && productDto.PhotoData.Length > 0)
        {
            var addPhotoResult =
                await photoService.AddPhotoAsync(new AddPhotoDto { PhotoData = productDto.PhotoData }, cancellationToken);
            if (addPhotoResult.IsFailed)
            {
                return Result.Fail(AppError.UnprocessableContent());
            }
            
            photoId = addPhotoResult.Value;
        }
        
        var product = new Product(productDto.Name, productDto.Description, productDto.Type, photoId);
        await productRepository.Update(id, product, cancellationToken);

        if (existingProduct.PhotoId is not null)
        {
            var deletePhotoResult = await photoService.DeletePhotoByIdAsync(existingProduct.PhotoId.Value, cancellationToken);
            if (deletePhotoResult.IsFailed)
            {
                return Result.Ok().WithError("Товар успешно обновлён, старое фото для удаления не найдено");
            }
        }
        
        return Result.Ok();
    }

    public async Task<Result> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var existingProduct = await productRepository.Get(id, cancellationToken);
        if (existingProduct is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        await productRepository.Delete(id, cancellationToken);

        if (existingProduct.PhotoId is not null)
        {
            var deletePhotoResult = await photoService.DeletePhotoByIdAsync(existingProduct.PhotoId.Value, cancellationToken);
            if (deletePhotoResult.IsFailed)
            {
                return Result.Ok().WithError("Товар успешно обновлён, старое фото для удаления не найдено");
            }
        }

        return Result.Ok();
    }
}