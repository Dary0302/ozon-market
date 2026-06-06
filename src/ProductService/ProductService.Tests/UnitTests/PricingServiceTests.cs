using FluentAssertions;
using Moq;
using NUnit.Framework;
using ProductService.Application.Services;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Tests.UnitTests;

public class PricingServiceTests
{
    private Mock<IPriceRepository> repositoryMock = null!;
    private PricingService service = null!;

    [SetUp]
    public void SetUp()
    {
        repositoryMock = new Mock<IPriceRepository>();
        service = new PricingService(repositoryMock.Object);
    }

    [Test]
    public async Task GetActualPrice_ShouldApplyDiscount()
    {
        var productId = Guid.NewGuid();

        repositoryMock
            .Setup(repository => repository.GetPrice(productId))
            .ReturnsAsync(new Price(productId, 100, 20));

        var result = await service.GetActualPrice(productId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(80);
    }

    [Test]
    public async Task GetActualPrice_ShouldFail_WhenPriceNotFound()
    {
        repositoryMock
            .Setup(repository => repository.GetPrice(It.IsAny<Guid>()))
            .ReturnsAsync((Price?)null);

        var result = await service.GetActualPrice(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldFail_WhenCostNegative()
    {
        var price = new Price(Guid.NewGuid(), -1, 10);

        var result = await service.SetPrice(price);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldFail_WhenDiscountLessOrEqualZero()
    {
        var price = new Price(Guid.NewGuid(), 100, 0);

        var result = await service.SetPrice(price);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldFail_WhenDiscountGreaterThan100()
    {
        var price = new Price(Guid.NewGuid(), 100, 101);

        var result = await service.SetPrice(price);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldSavePrice()
    {
        var price = new Price(Guid.NewGuid(), 100, 10);

        var result = await service.SetPrice(price);

        result.IsSuccess.Should().BeTrue();

        repositoryMock.Verify(repository => repository.SetPrice(price),
            Times.Once);
    }

    [Test]
    public async Task CalculateAmount_ShouldReturnCorrectAmount()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        repositoryMock
            .Setup(repository => repository.GetPrices(It.IsAny<List<Guid>>()))
            .ReturnsAsync([new Price(productId1, 100, 10), new Price(productId2, 50, 20)]);

        var result = await service.CalculateAmount(
            [new ProductQuantity(productId1, 2), new ProductQuantity(productId2, 1)]);

        result.IsSuccess.Should().BeTrue();

        // 100 * 0.9 * 2 + 50 * 0.8
        result.Value.Should().Be(220);
    }

    [Test]
    public async Task CalculateAmount_ShouldFail_WhenPriceMissing()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        repositoryMock
            .Setup(repository => repository.GetPrices(It.IsAny<List<Guid>>()))
            .ReturnsAsync([new Price(productId1, 100, 10)]);

        var result = await service.CalculateAmount(
            [new ProductQuantity(productId1, 1), new ProductQuantity(productId2, 1)]);

        result.IsFailed.Should().BeTrue();
    }
}