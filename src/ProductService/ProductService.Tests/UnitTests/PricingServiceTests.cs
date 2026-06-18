using Core.Common.Kafka.Contracts.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ProductService.Application.Services;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Tests.UnitTests;

public class PriceServiceTests
{
    private Mock<IPriceRepository> repositoryMock = null!;
    private PriceService service = null!;

    [SetUp]
    public void SetUp()
    {
        repositoryMock = new Mock<IPriceRepository>();
        service = new PriceService(repositoryMock.Object);
    }

    [Test]
    public async Task GetActualPrice_ShouldApplyDiscount()
    {
        var productId = Guid.NewGuid();

        repositoryMock
            .Setup(repository => repository.GetPrice(productId, DateTime.Now + TimeSpan.FromDays(1), CancellationToken.None))
            .ReturnsAsync(new Price(productId, 100, 20));

        var result = await service.GetActualPrice(productId, DateTime.Now + TimeSpan.FromDays(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(80);
    }

    [Test]
    public async Task GetActualPrice_ShouldFail_WhenPriceNotFound()
    {
        repositoryMock
            .Setup(repository => repository.GetPrice(It.IsAny<Guid>(), 
                It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync((Price?)null);

        var result = await service.GetActualPrice(Guid.NewGuid(), It.IsAny<DateTime>(), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldFail_WhenCostNegative()
    {
        var price = new Price(Guid.NewGuid(), -1, 10);

        var result = await service.SetPrice(price, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldFail_WhenDiscountLessOrEqualZero()
    {
        var price = new Price(Guid.NewGuid(), 100, 0);

        var result = await service.SetPrice(price, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldFail_WhenDiscountGreaterThan100()
    {
        var price = new Price(Guid.NewGuid(), 100, 101);

        var result = await service.SetPrice(price, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task SetPrice_ShouldSavePrice()
    {
        var price = new Price(Guid.NewGuid(), 100, 10);

        var result = await service.SetPrice(price, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        repositoryMock.Verify(repository => repository.SetPrice(price, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task CalculateAmount_ShouldReturnCorrectAmount()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        repositoryMock
            .Setup(repository => repository.GetPrices(It.IsAny<List<Guid>>(), It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync([new Price(productId1, 100, 10), new Price(productId2, 50, 20)]);

        var result = await service.CalculateAmount(
            [new ProductQuantity(productId1, 2), new ProductQuantity(productId2, 1)], CancellationToken.None);

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
            .Setup(repository => repository.GetPrices(It.IsAny<List<Guid>>(), It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync([new Price(productId1, 100, 10)]);

        var result = await service.CalculateAmount(
            [new ProductQuantity(productId1, 1), new ProductQuantity(productId2, 1)], CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }
}