using Core.Common.DbHelpers;
using Dapper;
using FluentAssertions;
using Npgsql;
using NUnit.Framework;
using ProductService.Domain;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Tests.IntegrationTests;

[TestFixture]
public class PriceRepositoryTests
{
    private PriceRepository priceRepository = null!;
    private ProductRepository productRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await using var connection =
            new NpgsqlConnection(PostgresFixture.Container.GetConnectionString());
        
        await connection.ExecuteAsync("""
                                      DELETE FROM prices;
                                      DELETE FROM products;
                                      """);

        var factory = new PostgresConnectionFactory(PostgresFixture.Container.GetConnectionString());

        priceRepository = new PriceRepository(factory);
        productRepository = new ProductRepository(factory);
    }

    [Test]
    public async Task SetPrice_And_GetPrice_ShouldReturnSavedPrice()
    {
        var product = new Product("Table",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        await productRepository.Add(product, CancellationToken.None);

        var price = new Price(product.Id, 100, 10) { Date = DateTime.UtcNow };

        await priceRepository.SetPrice(price, CancellationToken.None);

        var result = await priceRepository.GetPrice(product.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(price.Id);
        result.ProductId.Should().Be(product.Id);
        result.Cost.Should().Be(100);
        result.Discount.Should().Be(10);
    }

    [Test]
    public async Task GetPrice_ShouldReturnNull_WhenPriceDoesNotExist()
    {
        var result = await priceRepository.GetPrice(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Test]
    public async Task GetPrice_ShouldReturnLatestPrice()
    {
        var product = new Product("Table",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        await productRepository.Add(product, CancellationToken.None);

        var oldPrice = new Price(product.Id, 100, 0) { Date = DateTime.UtcNow.AddDays(-1) };
        var newPrice = new Price(product.Id, 200, 15) { Date = DateTime.UtcNow.AddDays(1) };

        await priceRepository.SetPrice(oldPrice, CancellationToken.None);
        await priceRepository.SetPrice(newPrice, CancellationToken.None);

        var result = await priceRepository.GetPrice(product.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(newPrice.Id);
        result.Cost.Should().Be(200);
        result.Discount.Should().Be(15);
    }

    [Test]
    public async Task GetPrices_ShouldReturnEmptyCollection_WhenProductsHaveNoPrices()
    {
        var result = await priceRepository.GetPrices([Guid.NewGuid(), Guid.NewGuid()]);

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetPrices_ShouldReturnPricesForRequestedProducts()
    {
        var product1 = new Product("Table",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        var product2 = new Product("Phone",
            "Description",
            ProductType.Phone,
            Guid.NewGuid());

        await productRepository.Add(product1, CancellationToken.None);
        await productRepository.Add(product2, CancellationToken.None);

        var price1 = new Price(product1.Id, 100, 5) { Date = DateTime.UtcNow };

        var price2 = new Price(product2.Id, 200, 15) { Date = DateTime.UtcNow };

        await priceRepository.SetPrice(price1, CancellationToken.None);
        await priceRepository.SetPrice(price2, CancellationToken.None);

        var result = (await priceRepository.GetPrices([product1.Id, product2.Id]))
            .ToList();

        result.Should().HaveCount(2);

        result.Should().Contain(price =>
            price!.ProductId == product1.Id &&
            price.Cost == 100);

        result.Should().Contain(price =>
            price!.ProductId == product2.Id &&
            price.Cost == 200);
    }

    [Test]
    public async Task GetPrices_ShouldReturnLatestPricesOnly()
    {
        var product = new Product("Phone",
            "Description",
            ProductType.Table,
            Guid.NewGuid());

        await productRepository.Add(product, CancellationToken.None);

        var oldPrice = new Price(product.Id, 100, 0) { Date = DateTime.UtcNow.AddDays(-1) };

        var latestPrice = new Price(product.Id, 300, 20) { Date = DateTime.UtcNow };

        await priceRepository.SetPrice(oldPrice, CancellationToken.None);
        await priceRepository.SetPrice(latestPrice, CancellationToken.None);

        var result = (await priceRepository.GetPrices([product.Id]))
            .Single();

        result.Should().NotBeNull();
        result.Id.Should().Be(latestPrice.Id);
        result.Cost.Should().Be(300);
        result.Discount.Should().Be(20);
    }
}