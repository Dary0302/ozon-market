using Core.Common.DbHelpers;
using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StorageService.Api.Controllers;
using StorageService.Api.Mappers;
using StorageService.Application.Dto;
using StorageService.Application.Implementations;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Domain;
using StorageService.Infrastructure.Implementations;
using StorageService.Infrastructure.Models;
using StorageService.Tests.Helpers;
using Xunit;

namespace StorageService.Tests.Api.Controllers;

public class StoredProductsControllerTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture fixture;
    private readonly StoredProductsController controller;
    private readonly IStoredProductRepository storedProductRepository;
    private readonly IStoragePointRepository storagePointRepository;
    private readonly IPvzPointRepository pvzPointRepository;
    private readonly PvzRepository pvzRepository;
    private readonly StorageRepository storageRepository;
    private readonly string connectionString;

    public StoredProductsControllerTests(PostgresFixture fixture)
    {
        this.fixture = fixture;
        this.connectionString = fixture.Container.GetConnectionString();
        var connectionFactory = new PostgresConnectionFactory(connectionString);
        
        storedProductRepository = new StoredProductRepository(connectionFactory);
        storagePointRepository = new StoragePointRepository(connectionFactory);
        pvzPointRepository = new PvzPointRepository(connectionFactory);
        pvzRepository = new PvzRepository(connectionFactory);
        storageRepository = new StorageRepository(connectionFactory);
        
        var service = new StoredProductService(
            storedProductRepository,
            storagePointRepository,
            pvzPointRepository,
            pvzRepository);
        
        controller = new StoredProductsController(service);
        
        // Очищаем таблицы перед каждым тестом
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.Execute("TRUNCATE TABLE stored_products CASCADE");
        connection.Execute("TRUNCATE TABLE storage_points CASCADE");
        connection.Execute("TRUNCATE TABLE pvz_points CASCADE");
        connection.Execute("TRUNCATE TABLE storages CASCADE");
        connection.Execute("TRUNCATE TABLE pvz CASCADE");
    }

    private async Task<Storage> CreateTestStorage()
    {
        var storage = Storage.Restore(Guid.NewGuid(), "Test Storage Address", Guid.NewGuid());
        await storageRepository.Add(storage, CancellationToken.None);
        return storage;
    }

    private async Task<Pvz> CreateTestPvz()
    {
        var pvz = Pvz.Restore(Guid.NewGuid(), "Test Pvz Address", Guid.NewGuid());
        await pvzRepository.Add(pvz, CancellationToken.None);
        return pvz;
    }

    private async Task<PvzPoint> CreateTestPvzPoint(Guid pvzId)
    {
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvzId, 55.751244, 37.618423);
        await pvzPointRepository.Add(pvzPoint, CancellationToken.None);
        return pvzPoint;
    }

    private async Task<StoragePoint> CreateTestStoragePoint(Guid storageId)
    {
        var storagePoint = StoragePoint.Restore(Guid.NewGuid(), storageId, 55.751244, 37.618423);
        await storagePointRepository.Add(storagePoint, CancellationToken.None);
        return storagePoint;
    }

    [Fact]
    public async Task CreateStoredProduct_ValidDto_ShouldReturnOk()
    {
        var storage = await CreateTestStorage();
        var dto = new AddStoredProductDto
        {
            ProductId = Guid.NewGuid(),
            StorageId = storage.Id,
            Quantity = 100
        };

        var result = await controller.CreateStoredProduct(dto, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task GetStoredProductsInStock_WhenProductsExist_ShouldReturnOkWithProducts()
    {
        var storage = await CreateTestStorage();
        
        var product1 = new StoredProduct(Guid.NewGuid(), storage.Id, 100);
        var product2 = new StoredProduct(Guid.NewGuid(), storage.Id, 50);
        var product3 = new StoredProduct(Guid.NewGuid(), storage.Id, 0);
        
        await storedProductRepository.Add(product1, CancellationToken.None);
        await storedProductRepository.Add(product2, CancellationToken.None);
        await storedProductRepository.Add(product3, CancellationToken.None);

        var result = await controller.GetStoredProductsInStock(CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(200);
        
        var returnedProducts = objectResult.Value as IEnumerable<ProductQuantityDto>;
        returnedProducts.Should().NotBeNull();
        returnedProducts.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStoredProductsInStock_WhenNoProducts_ShouldReturnOkWithEmptyList()
    {
        var result = await controller.GetStoredProductsInStock(CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(200);
        
        var returnedProducts = objectResult.Value as IEnumerable<ProductQuantityDto>;
        returnedProducts.Should().NotBeNull();
        returnedProducts.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDeliveryDate_WhenValidRequest_ShouldReturnOkWithDate()
    {
        // Создаём ПВЗ
        var pvz = await CreateTestPvz();
        
        // Создаём точку ПВЗ
        var pvzPoint = PvzPoint.Restore(pvz.PointId, pvz.Id, 55.751244, 37.618423);
        await pvzPointRepository.Add(pvzPoint, CancellationToken.None);
        
        // Создаём склад
        var storage = Storage.Restore(Guid.NewGuid(), "Test Storage", Guid.NewGuid());
        await storageRepository.Add(storage, CancellationToken.None);
        
        // Создаём точку склада
        var storagePoint = StoragePoint.Restore(storage.PointId, storage.Id, 55.751244, 37.618423);
        await storagePointRepository.Add(storagePoint, CancellationToken.None);
        
        // Добавляем товар
        var productId = Guid.NewGuid();
        var storedProduct = new StoredProduct(productId, storage.Id, 100);
        await storedProductRepository.Add(storedProduct, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId, 10)
        };

        var result = await controller.GetDeliveryDate(pvz.Id, orderedProducts, CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetDeliveryDate_WhenPvzNotFound_ShouldReturnNotFound()
    {
        var nonExistentPvzId = Guid.NewGuid();
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(Guid.NewGuid(), 10)
        };

        var result = await controller.GetDeliveryDate(nonExistentPvzId, orderedProducts, CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetDeliveryDate_WhenNotEnoughProducts_ShouldReturnUnprocessableEntity()
    {
        // Создаём ПВЗ
        var pvz = await CreateTestPvz();
        
        // Создаём точку ПВЗ
        var pvzPoint = PvzPoint.Restore(pvz.PointId, pvz.Id, 55.751244, 37.618423);
        await pvzPointRepository.Add(pvzPoint, CancellationToken.None);
        
        // Создаём склад
        var storage = Storage.Restore(Guid.NewGuid(), "Test Storage", Guid.NewGuid());
        await storageRepository.Add(storage, CancellationToken.None);
        
        // Создаём точку склада
        var storagePoint = StoragePoint.Restore(storage.PointId, storage.Id, 55.751244, 37.618423);
        await storagePointRepository.Add(storagePoint, CancellationToken.None);
        
        // Добавляем товар с малым количеством
        var productId = Guid.NewGuid();
        var storedProduct = new StoredProduct(productId, storage.Id, 10);
        await storedProductRepository.Add(storedProduct, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId, 100)
        };

        var result = await controller.GetDeliveryDate(pvz.Id, orderedProducts, CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(422);
    }

    [Fact]
    public async Task GetStoredProductRecords_WhenValidRequest_ShouldReturnOkWithRecords()
    {
        // Создаём ПВЗ
        var pvz = await CreateTestPvz();
        
        // Создаём точку ПВЗ
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        await pvzPointRepository.Add(pvzPoint, CancellationToken.None);
        
        // Создаём склад
        var storage = Storage.Restore(Guid.NewGuid(), "Test Storage", Guid.NewGuid());
        await storageRepository.Add(storage, CancellationToken.None);
        
        // Добавляем товары
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        
        var product1 = new StoredProduct(productId1, storage.Id, 100);
        var product2 = new StoredProduct(productId2, storage.Id, 50);
        
        await storedProductRepository.Add(product1, CancellationToken.None);
        await storedProductRepository.Add(product2, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId1, 30),
            new ProductQuantity(productId2, 20)
        };

        var result = await controller.GetStoredProductRecords(pvz.Id, orderedProducts, CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(200);
        
        var returnedRecords = objectResult.Value as List<DecreaseQuantityDto>;
        returnedRecords.Should().NotBeNull();
        returnedRecords.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStoredProductRecords_WhenPvzNotFound_ShouldReturnNotFound()
    {
        var nonExistentPvzId = Guid.NewGuid();
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(Guid.NewGuid(), 10)
        };

        var result = await controller.GetStoredProductRecords(nonExistentPvzId, orderedProducts, CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetStoredProductRecords_WhenEmptyOrderedProducts_ShouldReturnOkWithEmptyList()
    {
        var pvz = await CreateTestPvz();
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        await pvzPointRepository.Add(pvzPoint, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>();

        var result = await controller.GetStoredProductRecords(pvz.Id, orderedProducts, CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(200);
        
        var returnedRecords = objectResult.Value as List<DecreaseQuantityDto>;
        returnedRecords.Should().NotBeNull();
        returnedRecords.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStoredProductRecords_WhenProductsNotFound_ShouldReturnOkWithEmptyList()
    {
        var pvz = await CreateTestPvz();
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        await pvzPointRepository.Add(pvzPoint, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(Guid.NewGuid(), 30),
            new ProductQuantity(Guid.NewGuid(), 20)
        };

        var result = await controller.GetStoredProductRecords(pvz.Id, orderedProducts, CancellationToken.None);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(200);
        
        var returnedRecords = objectResult.Value as List<DecreaseQuantityDto>;
        returnedRecords.Should().NotBeNull();
        returnedRecords.Should().BeEmpty();
    }
}