using System.Data;
using Core.Common.DbHelpers.Interfaces;
using OrderService.Application;
using OrderService.Domain;
using FluentAssertions;
using Xunit;
using Moq;
using OrderService.Application.Implementations;
using OrderService.Application.Interfaces;
using OrderService.Application.Mocks;
using OrderService.Application.Models;
using OrderService.Domain.Exseptions;
using OrderService.Tests.Helpers;

namespace OrderService.Tests.Application;

public class OrderManagementServiceTests
{
    private readonly Mock<IOrderRepository> orderRepositoryMock;
    private readonly Mock<IOrderItemRepository> orderItemRepositoryMock;
    private readonly Mock<IOrderInfoRepository> orderInfoRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IStorageServiceMock> storageServiceMock;
    private readonly Mock<IProductServiceMock> productServiceMock;
    private readonly OrderManagementService service;

    public OrderManagementServiceTests()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        orderInfoRepositoryMock = new Mock<IOrderInfoRepository>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        storageServiceMock = new Mock<IStorageServiceMock>();
        productServiceMock = new Mock<IProductServiceMock>();
        
        unitOfWorkMock
            .Setup(uof => uof.ExecuteInTransaction(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(fn => fn());
        
        service = new OrderManagementService(
            orderRepositoryMock.Object, 
            orderItemRepositoryMock.Object, 
            orderInfoRepositoryMock.Object, 
            unitOfWorkMock.Object,
            storageServiceMock.Object,
            productServiceMock.Object);
    }
    
    private void SetupValidDefaultsForCreate(decimal calculatedAmount = 1000m)
    {
        productServiceMock
            .Setup(serviceMock => serviceMock.CalculateAmount(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(calculatedAmount);

        storageServiceMock
            .Setup(serviceMock => serviceMock.CheckStock(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(Enumerable.Empty<StockCheckResult>());

        storageServiceMock
            .Setup(serviceMock => serviceMock.GetDeliveryDate(
                It.IsAny<Guid>(), 
                It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(DateTime.UtcNow.AddDays(3));

        storageServiceMock
            .Setup(serviceMock => serviceMock.GetProductStorage(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(Enumerable.Empty<ProductStorage>());

        storageServiceMock
            .Setup(serviceMock => serviceMock.ReduceCountOfProducts(It.IsAny<IEnumerable<DecreaseQuantity>>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupValidDefaultsForUpdateStatus(Order order)
    {
        orderRepositoryMock
            .Setup(repository => repository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(order);
        orderRepositoryMock
            .Setup(repository => repository.Save(It.IsAny<Order>(), 
                It.IsAny<IDbConnection>(),  
                It.IsAny<IDbTransaction>(),
                CancellationToken.None))
            .ReturnsAsync(order.Id);
    }

    private void SetupValidDefaultsForGetInfo(Order order, IEnumerable<OrderItem> items)
    {
        orderRepositoryMock
            .Setup(repository => repository.GetById(order.Id, CancellationToken.None))
            .ReturnsAsync(order);
        orderItemRepositoryMock
            .Setup(repository => repository.GetAllByOrderId(order.Id, CancellationToken.None))
            .ReturnsAsync(items);
    }
    
    private static IEnumerable<ProductQuantity> MakeProducts(int count = 2) =>
        Enumerable.Range(1, count)
            .Select(_ => new ProductQuantity(Guid.NewGuid(), 1));

    
    #region Create
    
    [Fact]
    public async Task Create_ShouldReturnsSuccessWithOrderId()
    {
        // Arrange
        SetupValidDefaultsForCreate();
        var pvzId = Guid.NewGuid();
        var products = MakeProducts();
        
        // Act
        var result = await service.Create(pvzId, 1000m, products, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Create_ShouldCreateOrderAndItemsInTransaction()
    {
        // Arrange
        SetupValidDefaultsForCreate();
        var pvzId = Guid.NewGuid();
        var products = MakeProducts(3);
        
        // Act
        await service.Create(pvzId, 1000m, products, CancellationToken.None);
        
        // Assert
        orderRepositoryMock.Verify(
            repository => repository.Create(
                It.IsAny<Order>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(), 
                CancellationToken.None),
            Times.Once);

        orderItemRepositoryMock.Verify(
            repository => repository.Add(
                It.Is<List<OrderItem>>(items => items.Count() == 3),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(), 
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnsFail_IfInvalidClientAmount()
    {
        // Arrange
        SetupValidDefaultsForCreate();
        var clientAmount = 100m;
        var pvzId = Guid.NewGuid();
        var products = MakeProducts(3);

        // Act
        var result = await service.Create(pvzId, clientAmount, products, CancellationToken.None);

        // Assrert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == OrderErrors.InvalidAmount().Message);
    }

    [Fact]
    public async Task Create_ShouldReturnsFail_WhenStockError()
    {
        // Arrange
        SetupValidDefaultsForCreate();
        var lackingProduct = new LackingProduct(Guid.NewGuid(), 5);
        storageServiceMock
            .Setup(serviceMock => serviceMock.CheckStock(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(new[] { new StockCheckResult(lackingProduct.ProductId, -5) });
        
        // Act
        var result = await service.Create(Guid.NewGuid(), clientAmount: 1000m, 
            MakeProducts(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e
            => e.Message == OrderErrors.InsufficientStock(new[] { lackingProduct }).Message);
    }
    
    [Fact]
    public async Task Create_ShouldNormalizesAndSumsQuantities_WhenDuplicateProducts()
    {
        // Arrange
        SetupValidDefaultsForCreate();
        var sharedId = Guid.NewGuid();
        var products = new[]
        {
            new ProductQuantity(sharedId, 1),
            new ProductQuantity(sharedId, 1)
        };

        // Act
        await service.Create(Guid.NewGuid(), clientAmount: 1000m, products, CancellationToken.None);

        // Assert 
        productServiceMock.Verify(
            serviceMock => serviceMock.CalculateAmount(It.Is<IEnumerable<ProductQuantity>>(
                list => list.Count() == 1 && list.Single().Quantity == 2)),
            Times.Once);
    }
    
    #endregion
    
    #region GetById
    
    [Fact]
    public async Task GetById_ShouldReturnsSuccessWithOrderId()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        orderRepositoryMock
            .Setup(repository => repository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(order);
        
        // Act
        var result = await service.GetById(order.Id, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(order);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnsFail_WhenOrderDoesNotExist()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        orderRepositoryMock
            .Setup(repository => repository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Order?)null);
        
        // Act
        var result = await service.GetById(order.Id, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == OrderErrors.NotFound(order.Id).Message);
    }
    
    #endregion
    
    #region GetAll
    
    [Fact]
    public async Task GetAll_ShouldReturnsSuccessWithPagedOrders()
    {
        // Arrange
        var orders = Enumerable.Range(1, 3)
            .Select(_ => EntityFactory.MakeOrder())
            .ToList();
        var pagedResult = new PagedResult<Order>(orders, 3);
        
        orderRepositoryMock
            .Setup(repository => repository.GetAll(3, 10, CancellationToken.None))
            .ReturnsAsync(pagedResult);
        
        // Act
        var result = await service.GetAll(3, 10, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(pagedResult);
        result.Value.Items.Should().HaveCount(3);
        result.Value.Items.Should().BeEquivalentTo(pagedResult.Items);
        result.Value.TotalCount.Should().Be(3);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnsSuccessWithEmptyList_WhenNoOrders()
    {
        // Arrange
        var pagedResult = new PagedResult<Order>(Enumerable.Empty<Order>(), 0);
        
        orderRepositoryMock
            .Setup(repository => repository.GetAll(It.IsAny<int>(), 
                It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(pagedResult);
        
        // Act
        var result = await service.GetAll(3, 10, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
    
    #endregion
    
    #region UpdateStatus

    [Theory]
    [InlineData(Status.Created, Status.Paid)]
    [InlineData(Status.Paid, Status.InAssembly)]
    [InlineData(Status.InAssembly, Status.TransferredForDelivery)]
    [InlineData(Status.TransferredForDelivery, Status.Delivered)]
    public async Task UpdateStatus_ShouldReturnsSuccessWithOrderId_WhenTransitionIsValid(Status initialStatus, Status newStatus)
    {
        // Arrange
        var order = EntityFactory.MakeOrder(initialStatus);
        SetupValidDefaultsForUpdateStatus(order);

        // Act
        var result = await service.UpdateStatus(order.Id, newStatus, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(order.Id);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnsFail_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        orderRepositoryMock
            .Setup(repository => repository.GetById(orderId, CancellationToken.None))
            .ReturnsAsync((Order?)null);
        
        // Act
        var result = await service.UpdateStatus(orderId, Status.Paid, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == OrderErrors.NotFound(orderId).Message);
    }
    
    [Fact]
    public async Task UpdateStatus_ShouldReturnsFail_WhenInvalidStatusTransition()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        orderRepositoryMock
            .Setup(repository => repository.GetById(order.Id, CancellationToken.None))
            .ReturnsAsync(order);
        
        // Act
        var result = await service.UpdateStatus(order.Id, Status.InAssembly, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == OrderErrors.MustBePaid().Message);
    }
    
    [Theory]
    [InlineData(Status.InAssembly)]
    [InlineData(Status.TransferredForDelivery)]
    public async Task UpdateStatus_ShouldReturnsSuccess_WhenCancel(Status initialStatus)
    {
        // Arrange
        var order =  EntityFactory.MakeOrder(initialStatus);
        SetupValidDefaultsForUpdateStatus(order);

        // Act
        var result = await service.UpdateStatus(order.Id, Status.Canceled, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(order.Id);
    }

    [Theory]
    [InlineData(Status.Created)]
    [InlineData(Status.Delivered)]
    [InlineData(Status.Canceled)]
    public async Task UpdateStatus_ShouldReturnsSuccess_WhenCancelFromInvalidStatus(Status initialStatus)
    {
        // Arrange
        var order =  EntityFactory.MakeOrder(initialStatus);
        var expectedMessage = OrderErrors.InvalidStateForCancel(order.Status.ToString()).Message;
        SetupValidDefaultsForUpdateStatus(order);

        // Act
        var result = await service.UpdateStatus(order.Id, Status.Canceled, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == expectedMessage);
    }
    
    [Fact]
    public async Task UpdateStatus_ShouldNotSaveOrder_WhenTransitionIsInvalid()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        orderRepositoryMock
            .Setup(repository => repository.GetById(order.Id, CancellationToken.None))
            .ReturnsAsync(order);

        // Act
        await service.UpdateStatus(order.Id, Status.InAssembly, CancellationToken.None);

        // Assert
        orderRepositoryMock.Verify(
            repository => repository.Save(
                It.IsAny<Order>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(), 
                CancellationToken.None),
            Times.Never);
    }
    
    [Theory]
    [InlineData(Status.Created, Status.Paid)]
    [InlineData(Status.Paid, Status.InAssembly)]
    [InlineData(Status.InAssembly, Status.TransferredForDelivery)]
    [InlineData(Status.TransferredForDelivery, Status.Delivered)]
    [InlineData(Status.InAssembly, Status.Canceled)]
    [InlineData(Status.TransferredForDelivery, Status.Canceled)]
    public async Task UpdateStatus_ShouldSaveOrder_WhenTransitionIsValid(Status initialStatus, Status newStatus)
    {
        // Arrange
        var order = EntityFactory.MakeOrder(initialStatus);
        SetupValidDefaultsForUpdateStatus(order);

        // Act
        await service.UpdateStatus(order.Id, newStatus, CancellationToken.None);

        // Assert
        orderRepositoryMock.Verify(
            repository => repository.Save(
                It.IsAny<Order>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(), 
                CancellationToken.None),
            Times.Once);
    }
    
    #endregion
    
    #region Delete

    [Fact]
    public async Task Delete_ShouldReturnsSuccess()
    {
        // Arrange 
        var order = EntityFactory.MakeOrder();
        orderRepositoryMock
            .Setup(repository => repository.Delete(
                It.IsAny<Guid>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(), 
                CancellationToken.None));
        
        // Act
        var result = await service.Delete(order.Id, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    #endregion
    
    #region GetInfoById
    
    [Fact]
    public async Task GetInfoById_ShouldReturnsSuccess_WithOrderInfo()
    {
        // Arrange
        var orderInfo = EntityFactory.MakeOrderInfo();
        var productPrices = orderInfo.OrderItems
            .Select(item => new ProductPrice(item.ProductId, 100m, orderInfo.Order.CreatedOn))
            .ToList();
        var expectedOrderInfo = new OrderInfoWithPrice(
            orderInfo.Order,
            orderInfo.OrderItems.Select(item => new OrderItemWithPrice(
                item.ProductId,
                item.Quantity,
                100m)));
        SetupValidDefaultsForGetInfo(orderInfo.Order, orderInfo.OrderItems);
        productServiceMock
            .Setup(serviceMock => serviceMock.GetProductsPrice(It.IsAny<IEnumerable<ProductPriceRequest>>()))
            .ReturnsAsync(productPrices);

        // Act
        var result = await service.GetInfoById(orderInfo.Order.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue(result.Errors.FirstOrDefault()?.Message);
        result.Value.Should().BeEquivalentTo(expectedOrderInfo);
    }
    
    [Fact]
    public async Task GetInfoById_ShouldReturnsFail_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
    
        orderRepositoryMock
            .Setup(repository => repository.GetById(orderId, CancellationToken.None))
            .ReturnsAsync((Order?)null);
        orderItemRepositoryMock
            .Setup(repository => repository.GetAllByOrderId(orderId, CancellationToken.None))
            .ReturnsAsync(Enumerable.Empty<OrderItem>());

        // Act
        var result = await service.GetInfoById(orderId, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == OrderErrors.NotFound(orderId).Message);
    }
    
    #endregion
    
    #region GetAllInfo
    
    [Fact]
    public async Task GetAllInfo_ShouldReturnsSuccess_WithPagedOrderInfos()
    {
        // Arrange
        var orderInfos = Enumerable.Range(0, 3)
            .Select(_ => EntityFactory.MakeOrderInfo())
            .ToList();
        var pagedResult = new PagedResult<OrderInfo>(orderInfos, TotalCount: 3);
        var productPrices = orderInfos
            .SelectMany(orderInfo => orderInfo.OrderItems.Select(item => new ProductPrice(
                item.ProductId,
                100m,
                orderInfo.Order.CreatedOn))) 
            .ToList();
    
        orderInfoRepositoryMock
            .Setup(repository => repository.GetAll(1, 10, CancellationToken.None))
            .ReturnsAsync(pagedResult);
        productServiceMock
            .Setup(serviceMock => serviceMock.GetProductsPrice(It.IsAny<IEnumerable<ProductPriceRequest>>()))
            .ReturnsAsync(productPrices);

        // Act
        var result = await service.GetAllInfo(pageNumber: 1, pageSize: 10, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(3);
        result.Value.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetAllInfo_ShouldReturnsSuccess_WithEmptyList_WhenNoOrders()
    {
        // Arrange
        var pagedResult = new PagedResult<OrderInfo>(Enumerable.Empty<OrderInfo>(), TotalCount: 0);
    
        orderInfoRepositoryMock
            .Setup(repository => repository.GetAll(1, 10, CancellationToken.None))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await service.GetAllInfo(pageNumber: 1, pageSize: 10, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
    
    #endregion
}