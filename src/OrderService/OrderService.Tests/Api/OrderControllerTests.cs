using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Api.Controllers;
using OrderService.Api.Dto;
using OrderService.Api.Mappers;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Application.Simulation;
using OrderService.Domain;
using OrderService.Domain.Exseptions;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Api;

public class OrderControllerTests
{
    private readonly Mock<IOrderManagementService> serviceMock;
    private readonly Mock<IBackgroundSimulation> simulationBackgroundMock;
    private readonly OrderController controller;

    public OrderControllerTests()
    {
        serviceMock = new Mock<IOrderManagementService>();
        simulationBackgroundMock = new Mock<IBackgroundSimulation>();
        controller = new OrderController(serviceMock.Object, simulationBackgroundMock.Object);
    }
    
    #region CreateOrder
    
    [Fact]
    public async Task CreateOrder_ShouldReturns200_WhenSuccess()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var request = EntityFactory.MakeOrderRequestDto();

        serviceMock
            .Setup(service => service.Create(request.PvzId, request.ClientAmount, 
                request.Products, CancellationToken.None))
            .ReturnsAsync(Result.Ok(orderId));

        // Act
        var result = await controller.CreateOrder(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(orderId);
    }
    
    [Fact]
    public async Task CreateOrder_ShouldReturns422_WhenInvalidAmount()
    {
        // Arrange
        var request = EntityFactory.MakeOrderRequestDto();

        serviceMock
            .Setup(service => service.Create(request.PvzId, request.ClientAmount,
                request.Products, CancellationToken.None))
            .ReturnsAsync(Result.Fail(OrderErrors.InvalidAmount()));

        // Act
        var result = await controller.CreateOrder(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(409);
    }
    
    [Fact]
    public async Task CreateOrder_ShouldReturns422_WhenInsufficientStock()
    {
        // Arrange
        var request = EntityFactory.MakeOrderRequestDto();
        var lackingProduct = new LackingProduct(Guid.NewGuid(), 5);

        serviceMock
            .Setup(service => service.Create(request.PvzId, request.ClientAmount, 
                request.Products, CancellationToken.None))
            .ReturnsAsync(Result.Fail(OrderErrors.InsufficientStock(new[] { lackingProduct })));

        // Act
        var result = await controller.CreateOrder(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(409);
    }
    
    #endregion
    
    #region GetOrder

    [Fact]
    public async Task GetOrder_ShouldReturns200_WithOrderDto_WhenSuccess()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        var expectedResponse = new OrderResponseDto{
            Id = order.Id,
            PvzId = order.PvzId,
            Status = order.Status.ToString(),
            CreatedOn = order.CreatedOn,
            DeliveryDate = order.DeliveryDate,
            Amount = order.Amount};

        serviceMock
            .Setup(service => service.GetById(order.Id, CancellationToken.None))
            .ReturnsAsync(Result.Ok(order));

        // Act
        var result = await controller.GetOrder(order.Id, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task GetOrder_ShouldReturns404_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        serviceMock
            .Setup(service => service.GetById(orderId, CancellationToken.None))
            .ReturnsAsync(Result.Fail(OrderErrors.NotFound(orderId)));

        // Act
        var result = await controller.GetOrder(orderId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }
    
    #endregion

    #region PayOrder
    [Fact]
    public async Task PayOrder_ShouldReturns200_WhenSuccess()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        serviceMock
            .Setup(service => service.UpdateStatus(orderId, Status.Paid, CancellationToken.None))
            .ReturnsAsync(Result.Ok(orderId));

        // Act
        var result = await controller.PayOrder(orderId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(orderId);
    }
    
    [Fact]
    public async Task PayOrder_ShouldReturns404_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        serviceMock
            .Setup(service => service.UpdateStatus(orderId, Status.Paid, CancellationToken.None))
            .ReturnsAsync(Result.Fail(OrderErrors.NotFound(orderId)));

        // Act
        var result = await controller.PayOrder(orderId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task PayOrder_ShouldReturns409_WhenInvalidStatusTransition()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        serviceMock
            .Setup(service => service.UpdateStatus(orderId, Status.Paid, CancellationToken.None))
            .ReturnsAsync(Result.Fail(OrderErrors.InvalidStatusForPay()));

        // Act
        var result = await controller.PayOrder(orderId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(409);
    }
    
    #endregion
    
    #region GetOrderInfo
    
    [Fact]
    public async Task GetOrderInfo_ShouldReturns200_WithOrderInfoDto_WhenSuccess()
    {
        // Arrange
        var orderInfo = EntityFactory.MakeOrderInfoWithPrice();
        var expectedDto = new OrderInfoResponseDto
        {
            Id = orderInfo.Order.Id,
            PvzId = orderInfo.Order.PvzId,
            Status = orderInfo.Order.Status.ToString(),
            DeliveryDate = orderInfo.Order.DeliveryDate,
            Amount = orderInfo.Order.Amount,
            Products = orderInfo.OrderItems.Select(item => item.ToHttp())
        };

        serviceMock
            .Setup(service => service.GetInfoById(orderInfo.Order.Id, CancellationToken.None))
            .ReturnsAsync(Result.Ok(orderInfo));

        // Act
        var result = await controller.GetOrderInfo(orderInfo.Order.Id, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedDto);
    }
    
    [Fact]
    public async Task GetOrderInfo_ShouldReturns404_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        serviceMock
            .Setup(service => service.GetInfoById(orderId, CancellationToken.None))
            .ReturnsAsync(Result.Fail(OrderErrors.NotFound(orderId)));

        // Act
        var result = await controller.GetOrderInfo(orderId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }
    
    #endregion
    
    #region GetAllOrders

    [Fact]
    public async Task GetAllOrders_ShouldReturns200_WithPagedDto_WhenSuccess()
    {
        // Arrange
        var orders = Enumerable.Range(0, 3).Select(_ => EntityFactory.MakeOrder()).ToList();
        var pagedResult = new PagedResult<Order>(orders, TotalCount: 3);
        var expectedDto = new PagedResponseDto<OrderResponseDto>(
            orders.Select(o => o.ToHttp()),
            TotalCount: 3);
        var request = new PagedRequestDto (1, 10);

        serviceMock
            .Setup(service => service.GetAll(request.PageNumber, request.PageSize, CancellationToken.None))
            .ReturnsAsync(Result.Ok(pagedResult));

        // Act
        var result = await controller.GetAllOrders(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedDto);
    }
    
    #endregion
    
    #region GetAllOrdersInfo

    [Fact]
    public async Task GetAllOrdersInfo_ShouldReturns200_WithPagedDto_WhenSuccess()
    {
        // Arrange
        var orderInfos = Enumerable.Range(0, 3).Select(_ => EntityFactory.MakeOrderInfoWithPrice()).ToList();
        var pagedResult = new PagedResult<OrderInfoWithPrice>(orderInfos, TotalCount: 3);
        var expectedDto = new PagedResponseDto<OrderInfoResponseDto>(
            orderInfos.Select(o => o.ToHttp()),
            TotalCount: 3);
        var request = new PagedRequestDto (1, 10);

        serviceMock
            .Setup(service => service.GetAllInfo(request.PageNumber, request.PageSize, CancellationToken.None))
            .ReturnsAsync(Result.Ok(pagedResult));

        // Act
        var result = await controller.GetAllOrdersInfo(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedDto);
    }
    
    #endregion
    
    #region CancelOrder
    
    [Fact]
    public async Task CancelOrder_ShouldReturnsOk_WhenSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        serviceMock
            .Setup(s => s.Cancel(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(id));

        // Act
        var result = await controller.CancelOrder(id, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(id);
    }

    [Fact]
    public async Task CancelOrder_ShouldReturnsConflict_WhenInvalidState()
    {
        // Arrange
        var id = Guid.NewGuid();
        serviceMock
            .Setup(s => s.Cancel(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(OrderErrors.InvalidStateForCancel()));

        // Act
        var result = await controller.CancelOrder(id, CancellationToken.None);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }
    
    #endregion
    
    #region ChangeOrderStatus

    [Fact]
    public async Task ChangeOrderStatus_ShouldReturnsOk_WhenSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var newStatus = new StatusDto { Status = (int)Status.InAssembly };

        serviceMock
            .Setup(s => s.UpdateStatus(id, It.IsAny<Status>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(id));

        // Act
        var result = await controller.ChangeOrderStatus(id, newStatus, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        (result.Result as OkObjectResult)!.Value.Should().Be(id);
    }

    [Fact]
    public async Task ChangeOrderStatus_ShouldReturnsUnprocessableEntity_WhenFail()
    {
        // Arrange
        var id = Guid.NewGuid();
        var newStatus = new StatusDto { Status = (int)Status.InAssembly };

        serviceMock
            .Setup(s => s.UpdateStatus(id, It.IsAny<Status>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(OrderErrors.InvalidStatusTransition()));

        // Act
        var result = await controller.ChangeOrderStatus(id, newStatus, CancellationToken.None);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }
        
    #endregion
}