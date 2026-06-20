using FluentAssertions;
using FluentResults;
using Moq;
using NUnit.Framework;
using ProductService.Application.Dto;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Tests.UnitTests;

public class ProductManagementServiceTests
{
    private Mock<IProductRepository> productRepository = null!;
    private Mock<IPhotoService> photoService = null!;
    private ProductManagementService productService = null!;

    [SetUp]
    public void SetUp()
    {
        productRepository = new Mock<IProductRepository>();
        photoService = new Mock<IPhotoService>();

        productService = new ProductManagementService(productRepository.Object,
            photoService.Object);
    }

    [Test]
    public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
    {
        var product = new Product("Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        productRepository
            .Setup(repository => repository.Get(product.Id, CancellationToken.None))
            .ReturnsAsync(product);

        var result = await productService.GetProduct(product.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(product);
    }

    [Test]
    public async Task GetProduct_ShouldFail_WhenProductDoesNotExist()
    {
        productRepository
            .Setup(repository => repository.Get(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Product?)null);

        var result = await productService.GetProduct(Guid.NewGuid(), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task GetProducts_ShouldReturnProducts_WhenProductsFound()
    {
        var products = new List<Product?>
        {
            new("Phone",
                "Description",
                ProductType.Table,
                Guid.NewGuid())
        };

        productRepository
            .Setup(repository => repository.GetProductsByFilter(It.IsAny<ProductFilter>(), CancellationToken.None))
            .ReturnsAsync(products);

        var result = await productService.GetProducts(new ProductFilter(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Test]
    public async Task GetProducts_ShouldFail_WhenNothingFound()
    {
        productRepository
            .Setup(repository => repository.GetProductsByFilter(It.IsAny<ProductFilter>(), CancellationToken.None))
            .ReturnsAsync(Array.Empty<Product>());

        var result = await productService.GetProducts(new ProductFilter(), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task AddProduct_ShouldCreateProduct_WhenPhotoUploaded()
    {
        var photoId = Guid.NewGuid();

        photoService
            .Setup(service => service.AddPhotoAsync(It.IsAny<AddPhotoDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(photoId));

        var dto = new CreateProductDto
        {
            Name = "Phone", Description = "Description", Type = ProductType.Table, PhotoData = [1, 2, 3]
        };

        var result = await productService.AddProduct(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        productRepository.Verify(repository => repository.Add(It.Is<Product>(product =>
                product.Name == dto.Name &&
                product.Description == dto.Description &&
                product.Type == dto.Type &&
                product.PhotoId == photoId), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task AddProduct_ShouldFail_WhenPhotoUploadFailed()
    {
        photoService
            .Setup(service => service.AddPhotoAsync(It.IsAny<AddPhotoDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Guid>("error"));

        var dto = new CreateProductDto
        {
            Name = "Phone", Description = "Description", Type = ProductType.Table, PhotoData = [1, 2, 3]
        };

        var result = await productService.AddProduct(dto, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        productRepository.Verify(repository => repository.Add(It.IsAny<Product>(), CancellationToken.None),
            Times.Never);
    }

    [Test]
    public async Task UpdateProduct_ShouldFail_WhenProductNotFound()
    {
        productRepository
            .Setup(repository => repository.Get(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Product?)null);

        var dto = new CreateProductDto();

        var result = await productService.UpdateProduct(Guid.NewGuid(), dto, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        productRepository.Verify(repository => repository.Update(It.IsAny<Guid>(), It.IsAny<Product>(), CancellationToken.None),
            Times.Never);
    }

    [Test]
    public async Task UpdateProduct_ShouldSuccess_WhenOldPhotoDeleteFailed()
    {
        var existingProduct = new Product("Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        productRepository
            .Setup(repository => repository.Get(existingProduct.Id, CancellationToken.None))
            .ReturnsAsync(existingProduct);

        photoService
            .Setup(service => service.DeletePhotoByIdAsync(existingProduct.PhotoId!.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("error"));

        var result = await productService.UpdateProduct(existingProduct.Id,
            new CreateProductDto(), CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        productRepository.Verify(repository => repository.Update(It.IsAny<Guid>(), It.IsAny<Product>(), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateProduct_ShouldSuccess_WhenNewPhotoUploadFailed()
    {
        var existingProduct = new Product("Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        productRepository
            .Setup(repository => repository.Get(existingProduct.Id, CancellationToken.None))
            .ReturnsAsync(existingProduct);

        photoService
            .Setup(service => service.DeletePhotoByIdAsync(existingProduct.PhotoId!.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        photoService
            .Setup(service => service.AddPhotoAsync(It.IsAny<AddPhotoDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Guid>("error"));

        var result = await productService.UpdateProduct(existingProduct.Id,
            new CreateProductDto(), CancellationToken.None);

        result.IsFailed.Should().BeFalse();

        productRepository.Verify(repository => repository.Update(It.IsAny<Guid>(), It.IsAny<Product>(), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateProduct_ShouldUpdateProduct()
    {
        var existingProduct = new Product("Old",
            "OldDescription",
            ProductType.Table,
            Guid.NewGuid());

        var newPhotoId = Guid.NewGuid();

        productRepository
            .Setup(repository => repository.Get(existingProduct.Id, CancellationToken.None))
            .ReturnsAsync(existingProduct);

        photoService
            .Setup(service => service.DeletePhotoByIdAsync(existingProduct.PhotoId!.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        photoService
            .Setup(service => service.AddPhotoAsync(It.IsAny<AddPhotoDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(newPhotoId));

        var dto = new CreateProductDto
        {
            Name = "New", Description = "NewDescription", Type = ProductType.Phone, PhotoData = [1, 2, 3]
        };

        var result = await productService.UpdateProduct(existingProduct.Id, dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        productRepository.Verify(repository => repository.Update(existingProduct.Id,
                It.Is<Product>(product =>
                    product.Name == dto.Name &&
                    product.Description == dto.Description &&
                    product.Type == dto.Type &&
                    product.PhotoId == newPhotoId), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task DeleteProduct_ShouldFail_WhenProductNotFound()
    {
        productRepository
            .Setup(repository => repository.Get(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Product?)null);

        var result = await productService.DeleteProduct(Guid.NewGuid(), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task DeleteProduct_ShouldNotFail_WhenPhotoDeleteFailed()
    {
        var product = new Product("Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        productRepository
            .Setup(repository => repository.Get(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        photoService
            .Setup(service => service.DeletePhotoByIdAsync(product.PhotoId!.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("error"));

        var result = await productService.DeleteProduct(product.Id, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        productRepository.Verify(repository => repository.Delete(It.IsAny<Guid>(), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task DeleteProduct_ShouldDeleteProduct()
    {
        var product = new Product("Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        productRepository
            .Setup(repository => repository.Get(product.Id, CancellationToken.None))
            .ReturnsAsync(product);

        photoService
            .Setup(service => service.DeletePhotoByIdAsync(product.PhotoId!.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var result = await productService.DeleteProduct(product.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        productRepository.Verify(repository => repository.Delete(product.Id, CancellationToken.None),
            Times.Once);
    }
}