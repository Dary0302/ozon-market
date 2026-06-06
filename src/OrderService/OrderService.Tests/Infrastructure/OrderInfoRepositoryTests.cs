using Core.Common.DbHelpers;
using Dapper;
using FluentAssertions;
using Npgsql;
using OrderService.Application.Interfaces;
using OrderService.Domain;
using OrderService.Infrastructure.Implementations;
using OrderService.Infrastructure.Mappers;
using OrderService.Infrastructure.Models;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Infrastructure;

public class OrderInfoRepositoryTests : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly PostgresFixture postgresFixture;
    private readonly IOrderItemRepository orderItemRepository;
    private readonly IOrderRepository orderRepository;
    private readonly IOrderInfoRepository orderInfoRepository;

    public OrderInfoRepositoryTests(PostgresFixture fixture)
    {
        postgresFixture = fixture;
        var factory = new PostgresConnectionFactory(postgresFixture.Container.GetConnectionString());
        orderItemRepository = new OrderItemRepository(factory);
        orderRepository = new OrderRepository(factory);
        orderInfoRepository = new OrderInfoRepository(factory);
    }
    
    public async Task InitializeAsync()
    {
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM orders");
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
    // --- GetAll ---

    [Fact]
    public async Task GetAll_ShouldReturnAllCorrectInfo()
    {
        // Arrange
        var order1 = EntityFactory.MakeOrder();
        var items1 = Enumerable.Range(0, 5)
            .Select(_ => EntityFactory.MakeOrderItem(order1.Id))
            .ToList();
        var order2 = EntityFactory.MakeOrder();
        var items2 = Enumerable.Range(0, 5)
            .Select(_ => EntityFactory.MakeOrderItem(order2.Id))
            .ToList();
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await orderRepository.Create(order1, connection, null!, CancellationToken.None);
        await orderItemRepository.Add(items1,  connection, null!, CancellationToken.None);
        await orderRepository.Create(order2, connection, null!, CancellationToken.None);
        await orderItemRepository.Add(items2,  connection, null!, CancellationToken.None);
        
        // Act
        var result = await orderInfoRepository.GetAll(1, 10, CancellationToken.None);
        
        // Assert
        var info1 = result.Items.First(x => x.Order.Id == order1.Id);
        info1.OrderItems.Should().HaveCount(5);
        info1.OrderItems.Select(i => i.ProductId).Should()
            .BeEquivalentTo(items1.Select(i => i.ProductId));

        var info2 = result.Items.First(x => x.Order.Id == order2.Id);
        info2.OrderItems.Should().HaveCount(5);
        info2.OrderItems.Select(i => i.ProductId).Should()
            .BeEquivalentTo(items2.Select(i => i.ProductId));
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnPagedInfo()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        for (int i = 0; i < 5; i++)
        {
            var order = EntityFactory.MakeOrder();
            var items = Enumerable.Range(0, 3)
                .Select(_ => EntityFactory.MakeOrderItem(order.Id))
                .ToList();
            await orderRepository.Create(order, connection, null!, CancellationToken.None);
            await orderItemRepository.Add(items, connection, null!, CancellationToken.None);
        }
        
        // Act
        var result = await orderInfoRepository.GetAll(1, 3, CancellationToken.None);
        
        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(5);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnSecondPage()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        for (int i = 0; i < 5; i++)
        {
            var order = EntityFactory.MakeOrder();
            var items = Enumerable.Range(0, 3)
                .Select(_ => EntityFactory.MakeOrderItem(order.Id))
                .ToList();
            await orderRepository.Create(order, connection, null!, CancellationToken.None);
            await orderItemRepository.Add(items, connection, null!, CancellationToken.None);
        }
        
        // Act
        var result = await orderInfoRepository.GetAll(2, 3, CancellationToken.None);
        
        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnOrdersSortedByCreatedOnDesc()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        for (int i = 0; i < 5; i++)
        {
            var order = EntityFactory.MakeOrder();
            var items = Enumerable.Range(0, 3)
                .Select(_ => EntityFactory.MakeOrderItem(order.Id))
                .ToList();
            await orderRepository.Create(order, connection, null!, CancellationToken.None);
            await orderItemRepository.Add(items, connection, null!, CancellationToken.None);
        }
        
        // Act
        var result = await orderInfoRepository.GetAll(1, 10, CancellationToken.None);
        
        // Assert
        result.Items.Should().BeInDescendingOrder(orderInfo => orderInfo.Order.CreatedOn);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyPage_WhenNoOrders()
    {
        // Act
        var result = await orderInfoRepository.GetAll(1, 10, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}