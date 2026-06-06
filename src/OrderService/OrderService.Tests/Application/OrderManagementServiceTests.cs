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
            .Setup(u => u.ExecuteInTransaction(It.IsAny<Func<Task>>()))
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
            .Setup(p => p.CalculateAmount(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(calculatedAmount);

        storageServiceMock
            .Setup(s => s.CheckStock(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(Enumerable.Empty<StockCheckResult>());

        storageServiceMock
            .Setup(s => s.GetDeliveryDate(
                It.IsAny<Guid>(), 
                It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(DateTime.UtcNow.AddDays(3));

        storageServiceMock
            .Setup(s => s.GetProductStorage(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(Enumerable.Empty<ProductStorage>());

        storageServiceMock
            .Setup(s => s.ReduceCountOfProducts(It.IsAny<IEnumerable<DecreaseQuantity>>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupValidDefaultsForUpdateStatus(Order order)
    {
        orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(order);
        orderRepositoryMock
            .Setup(r => r.Save(It.IsAny<Order>(), 
                It.IsAny<IDbConnection>(),  
                It.IsAny<IDbTransaction>()))
            .ReturnsAsync(order.Id);
    }

    private void SetupValidDefaultsForGetInfo(Order order, IEnumerable<OrderItem> items)
    {
        orderRepositoryMock
            .Setup(r => r.GetById(order.Id))
            .ReturnsAsync(order);
        orderItemRepositoryMock
            .Setup(r => r.GetAllByOrderId(order.Id))
            .ReturnsAsync(items);
    }
    
    private static IEnumerable<ProductQuantity> MakeProducts(int count = 2) =>
        Enumerable.Range(1, count)
            .Select(_ => new ProductQuantity(Guid.NewGuid(), 1));

    
    // --- Create ---
    
    [Fact]
    public async Task Create_ShouldReturnsSuccessWithOrderId()
    {
        // Arrange
        SetupValidDefaultsForCreate();
        var pvzId = Guid.NewGuid();
        var products = MakeProducts();
        
        // Act
        var result = await service.Create(pvzId, 1000m, products);
        
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
        await service.Create(pvzId, 1000m, products);
        
        // Assert
        orderRepositoryMock.Verify(
            r => r.Create(
                It.IsAny<Order>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>()),
            Times.Once);

        orderItemRepositoryMock.Verify(
            r => r.Add(
                It.Is<List<OrderItem>>(items => items.Count() == 3),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>()),
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
        var result = await service.Create(pvzId, clientAmount, products);

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
            .Setup(s => s.CheckStock(It.IsAny<IEnumerable<ProductQuantity>>()))
            .ReturnsAsync(new[] { new StockCheckResult(lackingProduct.ProductId, -5) });
        
        // Act
        var result = await service.Create(Guid.NewGuid(), clientAmount: 1000m, MakeProducts());

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
        await service.Create(Guid.NewGuid(), clientAmount: 1000m, products);

        // Assert 
        productServiceMock.Verify(
            p => p.CalculateAmount(It.Is<IEnumerable<ProductQuantity>>(
                list => list.Count() == 1 && list.Single().Quantity == 2)),
            Times.Once);
    }
    
    // --- GetById ---
    
    [Fact]
    public async Task GetById_ShouldReturnsSuccessWithOrderId()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(order);
        
        // Act
        var result = await service.GetById(order.Id);
        
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
            .Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync((Order?)null);
        
        // Act
        var result = await service.GetById(order.Id);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == OrderErrors.NotFound(order.Id).Message);
    }
    
    // --- GetAll ---
    
    [Fact]
    public async Task GetAll_ShouldReturnsSuccessWithPagedOrders()
    {
        // Arrange
        var orders = Enumerable.Range(1, 3)
            .Select(_ => EntityFactory.MakeOrder())
            .ToList();
        var pagedResult = new PagedResult<Order>(orders, 3);
        
        orderRepositoryMock
            .Setup(r => r.GetAll(3, 10))
            .ReturnsAsync(pagedResult);
        
        // Act
        var result = await service.GetAll(3, 10);
        
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
            .Setup(r => r.GetAll(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(pagedResult);
        
        // Act
        var result = await service.GetAll(3, 10);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
    
    // --- UpdateStatus ---

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
        var result = await service.UpdateStatus(order.Id, newStatus);

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
            .Setup(r => r.GetById(orderId))
            .ReturnsAsync((Order?)null);
        
        // Act
        var result = await service.UpdateStatus(orderId, Status.Paid);
        
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
            .Setup(r => r.GetById(order.Id))
            .ReturnsAsync(order);
        
        // Act
        var result = await service.UpdateStatus(order.Id, Status.InAssembly);
        
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
        var result = await service.UpdateStatus(order.Id, Status.Canceled);

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
        var result = await service.UpdateStatus(order.Id, Status.Canceled);

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
            .Setup(r => r.GetById(order.Id))
            .ReturnsAsync(order);

        // Act
        await service.UpdateStatus(order.Id, Status.InAssembly);

        // Assert
        orderRepositoryMock.Verify(
            r => r.Save(
                It.IsAny<Order>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>()),
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
        await service.UpdateStatus(order.Id, newStatus);

        // Assert
        orderRepositoryMock.Verify(
            r => r.Save(
                It.IsAny<Order>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>()),
            Times.Once);
    }
    
    // --- Delete ---

    [Fact]
    public async Task Delete_ShouldReturnsSuccess()
    {
        // Arrange 
        var order = EntityFactory.MakeOrder();
        orderRepositoryMock
            .Setup(r => r.Delete(
                It.IsAny<Guid>(),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>()));
        
        // Act
        var result = await service.Delete(order.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    // --- GetInfoById ---
    
    [Fact]
    public async Task GetInfoById_ShouldReturnsSuccess_WithOrderInfo()
    {
        // Arrange
        var orderInfo = EntityFactory.MakeOrderInfo();
        SetupValidDefaultsForGetInfo(orderInfo.Order, orderInfo.OrderItems);

        // Act
        var result = await service.GetInfoById(orderInfo.Order.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(orderInfo);
    }
    
    [Fact]
    public async Task GetInfoById_ShouldReturnsFail_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
    
        orderRepositoryMock
            .Setup(r => r.GetById(orderId))
            .ReturnsAsync((Order?)null);
        orderItemRepositoryMock
            .Setup(r => r.GetAllByOrderId(orderId))
            .ReturnsAsync(Enumerable.Empty<OrderItem>());

        // Act
        var result = await service.GetInfoById(orderId);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == OrderErrors.NotFound(orderId).Message);
    }
    
    // --- GetAllInfo ---
    
    [Fact]
    public async Task GetAllInfo_ShouldReturnsSuccess_WithPagedOrderInfos()
    {
        // Arrange
        var orderInfos = Enumerable.Range(0, 3)
            .Select(_ => EntityFactory.MakeOrderInfo())
            .ToList();
        var pagedResult = new PagedResult<OrderInfo>(orderInfos, TotalCount: 3);
    
        orderInfoRepositoryMock
            .Setup(r => r.GetAll(1, 10))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await service.GetAllInfo(pageNumber: 1, pageSize: 10);

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
            .Setup(r => r.GetAll(1, 10))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await service.GetAllInfo(pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
}