using Core.Common.DbHelpers;
using Dapper;
using FluentAssertions;
using Npgsql;
using StorageService.Domain;
using StorageService.Infrastructure.Implementations;
using StorageService.Infrastructure.Models;
using StorageService.Tests.Helpers;
using Xunit;

namespace StorageService.Tests.Infrastructure;

public class StoredProductRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture fixture;
    private readonly StoredProductRepository repository;
    private readonly StorageRepository storageRepository;
    private readonly string connectionString;

    public StoredProductRepositoryTests(PostgresFixture fixture)
    {
        this.fixture = fixture;
        this.connectionString = fixture.Container.GetConnectionString();
        var connectionFactory = new PostgresConnectionFactory(connectionString);
        repository = new StoredProductRepository(connectionFactory);
        storageRepository = new StorageRepository(connectionFactory);
        
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.Execute("TRUNCATE TABLE stored_products CASCADE");
        connection.Execute("TRUNCATE TABLE storages CASCADE");
    }

    private async Task<Storage> CreateTestStorage()
    {
        var storage = Storage.Restore(Guid.NewGuid(), "Test Storage Address", Guid.NewGuid());
        await storageRepository.Add(storage, CancellationToken.None);
        return storage;
    }

    private async Task<StoredProduct> CreateTestStoredProduct(Guid productId, Guid storageId, int quantity)
    {
        var storedProduct = new StoredProduct(productId, storageId, quantity);
        await repository.Add(storedProduct, CancellationToken.None);
        return storedProduct;
    }

    [Fact]
    public async Task Add_ValidStoredProduct_ShouldSaveToDatabase()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        var storedProduct = new StoredProduct(productId, storage.Id, 100);

        await repository.Add(storedProduct, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId = storedProduct.ProductId, storageId = storedProduct.StorageId });
        
        result.Should().NotBeNull();
        result.ProductId.Should().Be(storedProduct.ProductId);
        result.StorageId.Should().Be(storedProduct.StorageId);
        result.Quantity.Should().Be(storedProduct.Quantity);
    }

    [Fact]
    public async Task GetByOrderedProducts_WithValidProducts_ShouldReturnReducedQuantities()
    {
        var storage = await CreateTestStorage();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId1, storage.Id, 100);
        await CreateTestStoredProduct(productId2, storage.Id, 50);
        
        var orderedProducts = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(productId1, storage.Id, 30),
            new DecreaseQuantity(productId2, storage.Id, 20)
        };

        var result = await repository.GetByOrderedProducts(orderedProducts, CancellationToken.None);

        result.Should().HaveCount(2);
        var product1 = result.First(p => p.ProductId == productId1);
        var product2 = result.First(p => p.ProductId == productId2);
        
        product1.Quantity.Should().Be(70); 
        product2.Quantity.Should().Be(30); 
    }

    [Fact]
    public async Task GetByOrderedProducts_WithEmptyList_ShouldReturnEmptyList()
    {
        var orderedProducts = new List<DecreaseQuantity>();

        var result = await repository.GetByOrderedProducts(orderedProducts, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByOrderedProducts_WithNonExistentProducts_ShouldReturnEmptyList()
    {
        var orderedProducts = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(Guid.NewGuid(), Guid.NewGuid(), 10)
        };

        var result = await repository.GetByOrderedProducts(orderedProducts, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllInStock_ShouldReturnOnlyProductsWithPositiveQuantity()
    {
        var storage = await CreateTestStorage();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var productId3 = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId1, storage.Id, 100);
        await CreateTestStoredProduct(productId2, storage.Id, 0);
        await CreateTestStoredProduct(productId3, storage.Id, 50);

        var result = await repository.GetAllInStock(CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.ProductId == productId1 && p.Quantity > 0);
        result.Should().Contain(p => p.ProductId == productId3 && p.Quantity > 0);
        result.Should().NotContain(p => p.ProductId == productId2);
    }

    [Fact]
    public async Task GetAllInStock_WhenNoProductsInStock_ShouldReturnEmptyList()
    {
        var result = await repository.GetAllInStock(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductsQuantity_WithValidProductIds_ShouldReturnTotalQuantities()
    {
        var storage1 = await CreateTestStorage();
        var storage2 = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId, storage1.Id, 100);
        await CreateTestStoredProduct(productId, storage2.Id, 50);
        
        var productIds = new List<Guid> { productId };

        var result = await repository.GetProductsQuantity(productIds, CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().ProductId.Should().Be(productId);
        result.First().Quantity.Should().Be(150); 
    }

    [Fact]
    public async Task GetProductsQuantity_WithMultipleProductIds_ShouldReturnQuantities()
    {
        var storage = await CreateTestStorage();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var productId3 = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId1, storage.Id, 100);
        await CreateTestStoredProduct(productId2, storage.Id, 200);
        await CreateTestStoredProduct(productId3, storage.Id, 300);
        
        var productIds = new List<Guid> { productId1, productId2 };

        var result = await repository.GetProductsQuantity(productIds, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.ProductId == productId1 && p.Quantity == 100);
        result.Should().Contain(p => p.ProductId == productId2 && p.Quantity == 200);
    }

    [Fact]
    public async Task GetProductsQuantity_WithEmptyList_ShouldReturnEmptyList()
    {
        var productIds = new List<Guid>();

        var result = await repository.GetProductsQuantity(productIds, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductsStorages_ShouldReturnAllStoragesForProducts()
    {
        var storage1 = await CreateTestStorage();
        var storage2 = await CreateTestStorage();
        var productId = Guid.NewGuid();
    
        await CreateTestStoredProduct(productId, storage1.Id, 100);
        await CreateTestStoredProduct(productId, storage2.Id, 50);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId, 30) 
        };

        var result = await repository.GetProductsStorages(orderedProducts, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.StorageId == storage1.Id);
        result.Should().Contain(p => p.StorageId == storage2.Id);
    }

    [Fact]
    public async Task GetProductsStorages_WhenNotEnoughQuantity_ShouldReturnOnlySuitableStorages()
    {
        var storage1 = await CreateTestStorage();
        var storage2 = await CreateTestStorage();
        var productId = Guid.NewGuid();
    
        await CreateTestStoredProduct(productId, storage1.Id, 30);
        await CreateTestStoredProduct(productId, storage2.Id, 100);
        
        var orderedProducts = new List<ProductQuantity>
        {
            new ProductQuantity(productId, 80)
        };

        var result = await repository.GetProductsStorages(orderedProducts, CancellationToken.None);

        result.Should().HaveCount(1);
        result.Should().Contain(p => p.StorageId == storage2.Id);
        result.Should().NotContain(p => p.StorageId == storage1.Id);
    }

    [Fact]
    public async Task DecreaseCount_ShouldReduceProductQuantity()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId, storage.Id, 100);
        
        var orderedProducts = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(productId, storage.Id, 30)
        };

        await repository.DecreaseCount(orderedProducts, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId, storageId = storage.Id });
        
        result.Quantity.Should().Be(70);
    }

    [Fact]
    public async Task DecreaseCount_WithMultipleProducts_ShouldReduceQuantities()
    {
        var storage = await CreateTestStorage();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId1, storage.Id, 100);
        await CreateTestStoredProduct(productId2, storage.Id, 200);
        
        var orderedProducts = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(productId1, storage.Id, 30),
            new DecreaseQuantity(productId2, storage.Id, 50)
        };

        await repository.DecreaseCount(orderedProducts, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result1 = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId = productId1, storageId = storage.Id });
        var result2 = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId = productId2, storageId = storage.Id });
        
        result1.Quantity.Should().Be(70);
        result2.Quantity.Should().Be(150);
    }

    [Fact]
    public async Task IncreaseCount_ShouldIncreaseProductQuantity()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId, storage.Id, 100);
        
        var arrivedProducts = new List<IncreaseQuantity>
        {
            new IncreaseQuantity(productId, storage.Id, 30)
        };

        await repository.IncreaseCount(arrivedProducts, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId, storageId = storage.Id });
        
        result.Quantity.Should().Be(130);
    }

    [Fact]
    public async Task IncreaseCount_WithMultipleProducts_ShouldIncreaseQuantities()
    {
        var storage = await CreateTestStorage();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId1, storage.Id, 100);
        await CreateTestStoredProduct(productId2, storage.Id, 200);
        
        var arrivedProducts = new List<IncreaseQuantity>
        {
            new IncreaseQuantity(productId1, storage.Id, 30),
            new IncreaseQuantity(productId2, storage.Id, 50)
        };

        await repository.IncreaseCount(arrivedProducts, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result1 = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId = productId1, storageId = storage.Id });
        var result2 = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId = productId2, storageId = storage.Id });
        
        result1.Quantity.Should().Be(130);
        result2.Quantity.Should().Be(250);
    }

    [Fact]
    public async Task Delete_ShouldRemoveProductFromStorage()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId, storage.Id, 100);
        
        await repository.Delete(productId, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<StoredProductDao>(
            "SELECT product_id, storage_id, quantity FROM stored_products WHERE product_id = @productId AND storage_id = @storageId",
            new { productId, storageId = storage.Id });
        
        result.Should().BeNull();
    }

    [Fact]
    public async Task Add_DuplicateProductInSameStorage_ShouldThrowException()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId, storage.Id, 100);
        
        var duplicateProduct = new StoredProduct(productId, storage.Id, 50);

        var act = async () => await repository.Add(duplicateProduct, CancellationToken.None);
        
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ComplexScenario_ShouldWorkCorrectly()
    {
        var storage = await CreateTestStorage();
        var productId = Guid.NewGuid();
        
        await CreateTestStoredProduct(productId, storage.Id, 100);
        
        var inStock = await repository.GetAllInStock(CancellationToken.None);
        inStock.Should().HaveCount(1);
        
        var decreaseList = new List<DecreaseQuantity>
        {
            new DecreaseQuantity(productId, storage.Id, 30)
        };
        await repository.DecreaseCount(decreaseList, CancellationToken.None);
        
        var quantity = await repository.GetProductsQuantity(new List<Guid> { productId }, CancellationToken.None);
        quantity.First().Quantity.Should().Be(70);
        
        var increaseList = new List<IncreaseQuantity>
        {
            new IncreaseQuantity(productId, storage.Id, 20)
        };
        await repository.IncreaseCount(increaseList, CancellationToken.None);
        
        var finalQuantity = await repository.GetProductsQuantity(new List<Guid> { productId }, CancellationToken.None);
        finalQuantity.First().Quantity.Should().Be(90);
        
        await repository.Delete(productId, CancellationToken.None);
        
        var afterDelete = await repository.GetProductsQuantity(new List<Guid> { productId }, CancellationToken.None);
        afterDelete.Should().BeEmpty();
    }
}