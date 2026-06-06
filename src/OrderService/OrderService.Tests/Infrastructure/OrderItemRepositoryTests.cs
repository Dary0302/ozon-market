using Core.Common.DbHelpers;
using Dapper;
using FluentAssertions;
using Npgsql;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Implementations;
using OrderService.Infrastructure.Models;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Infrastructure;

public class OrderItemRepositoryTests : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly PostgresFixture postgresFixture;
    private readonly IOrderItemRepository orderItemRepository;
    private readonly IOrderRepository orderRepository;

    public OrderItemRepositoryTests(PostgresFixture fixture)
    {
        postgresFixture = fixture;
        var factory = new PostgresConnectionFactory(postgresFixture.Container.GetConnectionString());
        orderItemRepository = new OrderItemRepository(factory);
        orderRepository = new OrderRepository(factory);
    }
    
    public async Task InitializeAsync()
    {
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM order_items");
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
    // --- Add ---

    [Fact]
    public async Task Add_ShouldInsertItem_AndReturnOrderId()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        var items = Enumerable.Range(0, 5)
            .Select(_ => EntityFactory.MakeOrderItem(order.Id))
            .ToList();
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        
        // Act
        await orderRepository.Create(order, connection, null!, CancellationToken.None);
        var returnedId = await orderItemRepository.Add(items,  connection, null!, CancellationToken.None);
        var sql = "SELECT  * FROM order_items WHERE order_id = @id";
        var rows = await connection.QueryAsync<OrderItemDao>(sql, new { id = order.Id });
        
        // Assert
        Assert.Equal(order.Id, returnedId);
        rows.Should().HaveCount(5);
        rows.Select(row => row.ProductId).Should()
            .BeEquivalentTo(items.Select(i => i.ProductId));
    }
    
    [Fact]
    public async Task Add_ShouldThrowException_WhenOrderDoesNotExist()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        var items = Enumerable.Range(0, 5)
            .Select(_ => EntityFactory.MakeOrderItem(order.Id))
            .ToList();
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        
        // Act
        var action = async () => await orderItemRepository.Add(
            items,  connection, null!, CancellationToken.None);
        
        // Assert
        await action.Should().ThrowAsync();
    }
    
    // --- GetAllByOrderId ---

    [Fact]
    public async Task GetAllByOrderId_ShouldReturnAllItems()
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
        var rows = await orderItemRepository.GetAllByOrderId(order1.Id, CancellationToken.None);
        rows.Should().HaveCount(5);
        rows.Select(row => row.ProductId).Should()
            .BeEquivalentTo(items1.Select(i => i.ProductId));
    }
    
    [Fact]
    public async Task GetAllByOrderId_ShouldReturnNull_WhenItemsDoesNotExist()
    {
        // Arrange
        var order1 = EntityFactory.MakeOrder();
        var items1 = Enumerable.Range(0, 5)
            .Select(_ => EntityFactory.MakeOrderItem(order1.Id))
            .ToList();
        var order2 = EntityFactory.MakeOrder();
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await orderRepository.Create(order1, connection, null!, CancellationToken.None);
        await orderItemRepository.Add(items1,  connection, null!, CancellationToken.None);
        
        // Act
        var rows = await orderItemRepository.GetAllByOrderId(order2.Id, CancellationToken.None);
        rows.Should().BeEmpty();
    }
}