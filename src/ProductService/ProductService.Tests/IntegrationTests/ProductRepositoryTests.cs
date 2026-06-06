using Core.Common.DbHelpers;
using Dapper;
using FluentAssertions;
using Npgsql;
using NUnit.Framework;
using ProductService.Domain;
using ProductService.Infrastructure.Repositories;
using ProductService.Tests.IntegrationTests.LocalDb;

namespace ProductService.Tests.IntegrationTests;

public class ProductRepositoryTests
{
    private ProductRepository repository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await using var connection =
            new NpgsqlConnection(PostgresFixture.Container.GetConnectionString());

        await connection.ExecuteAsync("""
                                      DELETE FROM prices;
                                      DELETE FROM products;
                                      """);

        var factory = new PostgresConnectionFactory(
            PostgresFixture.Container.GetConnectionString());

        repository = new ProductRepository(factory);
    }

    [Test]
    public async Task Add_ShouldSaveProduct()
    {
        var product = new Product(
            "Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());
        
        await repository.Add(product);

        var saved = await repository.Get(product.Id);
        
        saved.Should().NotBeNull();
        saved.Id.Should().Be(product.Id);
    }
    
    [Test]
    public async Task Get_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var result = await repository.Get(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Test]
    public async Task Delete_ShouldRemoveProduct()
    {
        var product = new Product(
            "Phone",
            "Description",
            ProductType.Phone,
            Guid.NewGuid());

        await repository.Add(product);

        await repository.Delete(product.Id);

        var result = await repository.Get(product.Id);

        result.Should().BeNull();
    }

    [Test]
    public async Task Delete_ShouldNotThrow_WhenProductDoesNotExist()
    {
        var action = async () => await repository.Delete(Guid.NewGuid());

        await action.Should().NotThrowAsync();
    }
    
    [Test]
    public async Task Update_ShouldUpdateExistingProduct()
    {
        var product = new Product(
            "Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        await repository.Add(product);

        var updatedProduct = new Product(
            "Laptop",
            "Updated description",
            ProductType.Bed,
            Guid.NewGuid());

        await repository.Update(product.Id, updatedProduct);

        var result = await repository.Get(product.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be(updatedProduct.Name);
        result.Description.Should().Be(updatedProduct.Description);
        result.Type.Should().Be(updatedProduct.Type);
        result.PhotoId.Should().Be(updatedProduct.PhotoId);
    }

    [Test]
    public async Task Update_ShouldNotCreateNewProduct_WhenProductDoesNotExist()
    {
        var id = Guid.NewGuid();

        var product = new Product(
            "Phone",
            "Description",
            ProductType.Toy,
            Guid.NewGuid());

        await repository.Update(id, product);

        var result = await repository.Get(id);

        result.Should().BeNull();
    }
}