using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Api.Controllers;
using OrderService.Api.Dto;
using OrderService.Api.Mappers;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Domain.Exseptions;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Api;

public class OrderControllerTests
{
    private readonly Mock<IOrderManagementService> serviceMock;
    private readonly OrderController controller;

    public OrderControllerTests()
    {
        serviceMock = new Mock<IOrderManagementService>();
        controller = new OrderController(serviceMock.Object);
    }
    
    // --- CreateOrder ---
    
    [Fact]
    public async Task CreateOrder_ShouldReturns200_WhenSuccess()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var request = EntityFactory.MakeOrderRequestDto();

        serviceMock
            .Setup(s => s.Create(request.PvzId, request.ClientAmount, request.Products))
            .ReturnsAsync(Result.Ok(orderId));

        // Act
        var result = await controller.CreateOrder(request);

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
            .Setup(s => s.Create(request.PvzId, request.ClientAmount, request.Products))
            .ReturnsAsync(Result.Fail(OrderErrors.InvalidAmount()));

        // Act
        var result = await controller.CreateOrder(request);

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
            .Setup(s => s.Create(request.PvzId, request.ClientAmount, request.Products))
            .ReturnsAsync(Result.Fail(OrderErrors.InsufficientStock(new[] { lackingProduct })));

        // Act
        var result = await controller.CreateOrder(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(409);
    }
    
    // --- GetOrder ---

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
            .Setup(s => s.GetById(order.Id))
            .ReturnsAsync(Result.Ok(order));

        // Act
        var result = await controller.GetOrder(order.Id);

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
            .Setup(s => s.GetById(orderId))
            .ReturnsAsync(Result.Fail(OrderErrors.NotFound(orderId)));

        // Act
        var result = await controller.GetOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }
    
    // --- PayOrder ---

    [Fact]
    public async Task PayOrder_ShouldReturns200_WhenSuccess()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        serviceMock
            .Setup(s => s.UpdateStatus(orderId, Status.Paid))
            .ReturnsAsync(Result.Ok(orderId));

        // Act
        var result = await controller.PayOrder(orderId);

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
            .Setup(s => s.UpdateStatus(orderId, Status.Paid))
            .ReturnsAsync(Result.Fail(OrderErrors.NotFound(orderId)));

        // Act
        var result = await controller.PayOrder(orderId);

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
            .Setup(s => s.UpdateStatus(orderId, Status.Paid))
            .ReturnsAsync(Result.Fail(OrderErrors.MustBeCreated()));

        // Act
        var result = await controller.PayOrder(orderId);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(409);
    }
    
    // --- GetOrderInfo ---
    
    [Fact]
    public async Task GetOrderInfo_ShouldReturns200_WithOrderInfoDto_WhenSuccess()
    {
        // Arrange
        var orderInfo = EntityFactory.MakeOrderInfo();
        var expectedDto = new OrderInfoResponseDto
        {
            Id = orderInfo.Order.Id,
            PvzId = orderInfo.Order.PvzId,
            Status = orderInfo.Order.Status.ToString(),
            DeliveryDate = orderInfo.Order.DeliveryDate,
            Amount = orderInfo.Order.Amount,
            Products = orderInfo.OrderItems
        };

        serviceMock
            .Setup(s => s.GetInfoById(orderInfo.Order.Id))
            .ReturnsAsync(Result.Ok(orderInfo));

        // Act
        var result = await controller.GetOrderInfo(orderInfo.Order.Id);

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
            .Setup(s => s.GetInfoById(orderId))
            .ReturnsAsync(Result.Fail(OrderErrors.NotFound(orderId)));

        // Act
        var result = await controller.GetOrderInfo(orderId);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }
    
    // --- GetAllOrders ---

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
            .Setup(s => s.GetAll(request.PageNumber, request.PageSize))
            .ReturnsAsync(Result.Ok(pagedResult));

        // Act
        var result = await controller.GetAllOrders(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedDto);
    }
    
    // --- GetAllOrdersInfo ---

    [Fact]
    public async Task GetAllOrdersInfo_ShouldReturns200_WithPagedDto_WhenSuccess()
    {
        // Arrange
        var orderInfos = Enumerable.Range(0, 3).Select(_ => EntityFactory.MakeOrderInfo()).ToList();
        var pagedResult = new PagedResult<OrderInfo>(orderInfos, TotalCount: 3);
        var expectedDto = new PagedResponseDto<OrderInfoResponseDto>(
            orderInfos.Select(o => o.ToHttp()),
            TotalCount: 3);
        var request = new PagedRequestDto (1, 10);

        serviceMock
            .Setup(s => s.GetAllInfo(request.PageNumber, request.PageSize))
            .ReturnsAsync(Result.Ok(pagedResult));

        // Act
        var result = await controller.GetAllOrdersInfo(request);

        // Assert
        var dto = result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedDto);
    }
}