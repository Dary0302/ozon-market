using Core.Common.DbHelpers;
using FluentAssertions;
using Npgsql;
using Dapper;
using StorageService.Application.Dto;
using StorageService.Application.Implementations;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Domain;
using StorageService.Infrastructure.Implementations;
using StorageService.Infrastructure.Models;
using StorageService.Tests.Helpers;
using Xunit;

namespace StorageService.Tests.Application;

public class StoredProductServiceTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture fixture;
    private readonly StoredProductService service;
    private readonly IStoredProductRepository storedProductRepository;
    private readonly IStoragePointRepository storagePointRepository;
    private readonly IPvzPointRepository pvzPointRepository;
    private readonly StorageRepository storageRepository;
    private readonly PvzRepository pvzRepository;
    private readonly string connectionString;

    public StoredProductServiceTests(PostgresFixture fixture)
    {
        this.fixture = fixture;
        this.connectionString = fixture.Container.GetConnectionString();
        var connectionFactory = new PostgresConnectionFactory(connectionString);
        
        storedProductRepository = new StoredProductRepository(connectionFactory);
        storagePointRepository = new StoragePointRepository(connectionFactory);
        pvzPointRepository = new PvzPointRepository(connectionFactory);
        storageRepository = new StorageRepository(connectionFactory);
        pvzRepository = new PvzRepository(connectionFactory);
        
        service = new StoredProductService(
            storedProductRepository,
            storagePointRepository,
            pvzPointRepository,
            pvzRepository,
            storageRepository);
        
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
    public async Task AddStoredProduct_ValidDto_ShouldSaveToDatabase()
    {
        var storage = await CreateTestStorage();
        var storedProduct = new StoredProduct(Guid.NewGuid(), storage.Id, 100);

        var result = await service.AddStoredProduct(storedProduct, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        
        using var connection = new NpgsqlConnection(connectionString);
        var savedProduct = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId = storedProduct.ProductId, storageId = storedProduct.StorageId });
        
        savedProduct.Should().NotBeNull();
        savedProduct.ProductId.Should().Be(storedProduct.ProductId);
        savedProduct.StorageId.Should().Be(storedProduct.StorageId);
        savedProduct.Quantity.Should().Be(storedProduct.Quantity);
    }

    [Fact]
    public async Task GetStoredProductsInStock_ShouldReturnOnlyProductsWithPositiveQuantity()
    {
        var storage = await CreateTestStorage();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var productId3 = Guid.NewGuid();
        
        var product1 = new StoredProduct(productId1, storage.Id, 100);
        var product2 = new StoredProduct(productId2, storage.Id, 0);
        var product3 = new StoredProduct(productId3, storage.Id, 50);
        
        await storedProductRepository.Add(product1, CancellationToken.None);
        await storedProductRepository.Add(product2, CancellationToken.None);
        await storedProductRepository.Add(product3, CancellationToken.None);

        var result = await service.GetStoredProductsInStock(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(p => p.ProductId == productId1 && p.Quantity == 100);
        result.Value.Should().Contain(p => p.ProductId == productId3 && p.Quantity == 50);
        result.Value.Should().NotContain(p => p.ProductId == productId2);
    }

    [Fact]
    public async Task CheckStock_WhenAllProductsHaveEnoughQuantity_ShouldReturnStockCheckResults()
    {
        var storage = await CreateTestStorage();
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

        var result = await service.CheckStock(orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(r => r.ProductId == productId1 && r.Difference == 70);
        result.Value.Should().Contain(r => r.ProductId == productId2 && r.Difference == 30);
    }

    [Fact]
    public async Task CheckStock_WhenProductNotFound_ShouldReturnFail()
    {
        var storage = await CreateTestStorage();
        var existingProductId = Guid.NewGuid();
        var missingProductId = Guid.NewGuid();
        
        var existingProduct = new StoredProduct(existingProductId, storage.Id, 100);
        await storedProductRepository.Add(existingProduct, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(existingProductId, 30),
            new ProductQuantity(missingProductId, 20)
        };

        var result = await service.CheckStock(orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("не совпадает"));
    }

    [Fact]
    public async Task CheckStock_WhenNotEnoughQuantity_ShouldReturnFail()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        var product = new StoredProduct(productId, storage.Id, 50);
        await storedProductRepository.Add(product, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId, 100)
        };

        var result = await service.CheckStock(orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Не хватает товара на складе"));
    }

    [Fact]
    public async Task GetDeliveryDate_WhenPvzNotFound_ShouldReturnNotFound()
    {
        var pvzId = Guid.NewGuid();
        var orderedProducts = new List<ProductQuantity>();

        var result = await service.GetDeliveryDate(pvzId, orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Пвз не найден"));
    }

    [Fact]
    public async Task GetDeliveryDate_WhenPvzFound_ShouldReturnDeliveryDate()
    {
        var pvz = Pvz.Restore(Guid.NewGuid(), "Test Pvz Address", Guid.NewGuid());
        await pvzRepository.Add(pvz, CancellationToken.None);
        
        var pvzPoint = PvzPoint.Restore(pvz.PointId, pvz.Id, 55.751244, 37.618423);
        await pvzPointRepository.Add(pvzPoint, CancellationToken.None);
        
        var storage = Storage.Restore(Guid.NewGuid(), "Test Storage Address", Guid.NewGuid());
        await storageRepository.Add(storage, CancellationToken.None);
        
        var storagePoint = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        await storagePointRepository.Add(storagePoint, CancellationToken.None);
        
        var productId = Guid.NewGuid();
        var storedProduct = new StoredProduct(productId, storage.Id, 100);
        await storedProductRepository.Add(storedProduct, CancellationToken.None);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId, 10)
        };
        
        var foundPvz = await pvzRepository.Get(pvz.Id, CancellationToken.None);
        foundPvz.Should().NotBeNull();
        
        var foundPvzPoint = await pvzPointRepository.Get(pvz.PointId, CancellationToken.None);
        foundPvzPoint.Should().NotBeNull();
        
        var foundProducts = await storedProductRepository.GetProductsQuantity(new[] { productId }, CancellationToken.None);
        foundProducts.Should().NotBeEmpty();
        
        var foundStoragePoints = await storagePointRepository.GetStoragePoints(new[] { storage.Id }, CancellationToken.None);
        foundStoragePoints.Should().NotBeEmpty();
        
        var productsStorages = await service.GetProductsStorages(orderedProducts, CancellationToken.None);
        productsStorages.Should().NotBeEmpty();
        
        var orderStorages = await service.GetOrderStorages(productsStorages.ToList(), CancellationToken.None);
        orderStorages.Should().NotBeEmpty();
        
        var result = await service.GetDeliveryDate(pvz.Id, orderedProducts, CancellationToken.None);
        
        if (!result.IsSuccess)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Message));
            throw new Exception($"Ошибка: {errors}");
        }
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeAfter(DateTime.Now.AddMilliseconds(-100));
    }

    [Fact]
    public async Task GetOrderStoragesRecords_WhenPvzNotFound_ShouldReturnNotFound()
    {
        var pvzId = Guid.NewGuid();
        var orderedProducts = new List<ProductQuantity>();

        var result = await service.GetOrderStoragesRecords(pvzId, orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Пвз не найден"));
    }

    [Fact]
    public async Task GetOrderStoragesRecords_WhenPvzFound_ShouldReturnDecreaseQuantities()
    {
        var pvz = await CreateTestPvz();
        await CreateTestPvzPoint(pvz.Id);
        
        var storage = await CreateTestStorage();
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

        var result = await service.GetOrderStoragesRecords(pvz.Id, orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(d => d.ProductId == productId1 && d.StorageId == storage.Id);
        result.Value.Should().Contain(d => d.ProductId == productId2 && d.StorageId == storage.Id);
    }

    [Fact]
    public async Task DecreaseStoredProductQuantity_WhenEnoughQuantity_ShouldDecrease()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        var storedProduct = new StoredProduct(productId, storage.Id, 100);
        await storedProductRepository.Add(storedProduct, CancellationToken.None);
        
        var orderedProducts = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(productId, storage.Id, 30)
        };

        var result = await service.DecreaseStoredProductQuantity(orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        
        using var connection = new NpgsqlConnection(connectionString);
        var updatedProduct = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId, storageId = storage.Id });
        
        updatedProduct.Quantity.Should().Be(70);
    }

    [Fact]
    public async Task DecreaseStoredProductQuantity_WhenNotEnoughQuantity_ShouldReturnFail()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        var storedProduct = new StoredProduct(productId, storage.Id, 100);
        await storedProductRepository.Add(storedProduct, CancellationToken.None);
        
        var orderedProducts = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(productId, storage.Id, 150)
        };

        var result = await service.DecreaseStoredProductQuantity(orderedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Не хватает товара на складе"));
        
        using var connection = new NpgsqlConnection(connectionString);
        var updatedProduct = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId, storageId = storage.Id });
        
        updatedProduct.Quantity.Should().Be(100);
    }

    [Fact]
    public async Task IncreaseStoredProductQuantity_ShouldIncrease()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        var storedProduct = new StoredProduct(productId, storage.Id, 100);
        await storedProductRepository.Add(storedProduct, CancellationToken.None);
        
        var arrivedProducts = new List<IncreaseQuantity>
        {
            new IncreaseQuantity(productId, storage.Id, 50)
        };

        var result = await service.IncreaseStoredProductQuantity(arrivedProducts, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        
        using var connection = new NpgsqlConnection(connectionString);
        var updatedProduct = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId, storageId = storage.Id });
        
        updatedProduct.Quantity.Should().Be(150);
    }

    [Fact]
    public async Task ComplexScenario_AddCheckDecreaseIncrease_ShouldWorkCorrectly()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        var storedProduct = new StoredProduct(productId, storage.Id, 100);
        
        await service.AddStoredProduct(storedProduct, CancellationToken.None);
        
        var inStock = await service.GetStoredProductsInStock(CancellationToken.None);
        inStock.Value.Should().Contain(p => p.ProductId == productId && p.Quantity == 100);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId, 30)
        };
        
        var checkResult = await service.CheckStock(orderedProducts, CancellationToken.None);
        checkResult.IsSuccess.Should().BeTrue();
        
        var decreaseList = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(productId, storage.Id, 30)
        };
        
        await service.DecreaseStoredProductQuantity(decreaseList, CancellationToken.None);
        
        var afterDecrease = await service.GetStoredProductsInStock(CancellationToken.None);
        afterDecrease.Value.Should().Contain(p => p.ProductId == productId && p.Quantity == 70);
        
        var increaseList = new List<IncreaseQuantity>
        {
            new IncreaseQuantity(productId, storage.Id, 20)
        };
        
        await service.IncreaseStoredProductQuantity(increaseList, CancellationToken.None);
        
        var finalStock = await service.GetStoredProductsInStock(CancellationToken.None);
        finalStock.Value.Should().Contain(p => p.ProductId == productId && p.Quantity == 90);
    }
}