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

public class OrderRepositoryTests : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly PostgresFixture postgresFixture;
    private readonly IOrderRepository repository;

    public OrderRepositoryTests(PostgresFixture fixture)
    {
        postgresFixture = fixture;
        var factory = new PostgresConnectionFactory(postgresFixture.Container.GetConnectionString());
        repository = new OrderRepository(factory);
    }
    
    public async Task InitializeAsync()
    {
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM orders");
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
    [Fact]
    public async Task Create_ShouldInsertOrder_AndReturnId()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        
        // Act
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        var returnedId = await repository.Create(order, connection, null!);
        
        var sql = "SELECT  * FROM orders WHERE id = @id";
        var row = await connection.QueryFirstOrDefaultAsync<OrderDao>(sql, new { id = order.Id });

        //Assert
        Assert.Equal(order.Id, returnedId);
        row.ToDomain().Should().BeEquivalentTo(order,
            options => options
                .Excluding(x => x.DeliveryDate)
                .Excluding(x => x.CreatedOn));
        row.ToDomain().DeliveryDate.Should().BeCloseTo(order.DeliveryDate, TimeSpan.FromMilliseconds(1));
        row.ToDomain().CreatedOn.Should().BeCloseTo(order.CreatedOn, TimeSpan.FromMilliseconds(1));
    }
    
    [Fact]
    public async Task Create_ShouldThrowException_WhenDuplicateId()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await repository.Create(order, connection, null!);
        
        // Act
        var action = async () => await repository.Create(order, connection, null!);
        
        // Assert 
        await action.Should().ThrowAsync();
    }
    
    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await repository.Create(order, connection, null!);
        
        // Act
        var result = await repository.GetById(order.Id);
        
        // Assert
        result.Should().BeEquivalentTo(order,
            options => options
                .Excluding(x => x.DeliveryDate)
                .Excluding(x => x.CreatedOn));
        result.DeliveryDate.Should().BeCloseTo(order.DeliveryDate, TimeSpan.FromMilliseconds(1));
        result.CreatedOn.Should().BeCloseTo(order.CreatedOn, TimeSpan.FromMilliseconds(1));
    }
    
    [Fact]
    public async Task GetById_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        
        // Act
        var result = await repository.GetById(id);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnPagedOrders()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
    
        var orders = Enumerable.Range(0, 5).Select(_ => EntityFactory.MakeOrder()).ToList();
        foreach (var order in orders)
            await repository.Create(order, connection, null!);

        // Act
        var result = await repository.GetAll(1, 3);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAll_ShouldReturnSecondPage()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
    
        var orders = Enumerable.Range(0, 5).Select(_ => EntityFactory.MakeOrder()).ToList();
        foreach (var order in orders)
            await repository.Create(order, connection, null!);

        // Act
        var result = await repository.GetAll(2, 3);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOrdersSortedByCreatedOnDesc()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
    
        var orders = Enumerable.Range(0, 3).Select(_ => EntityFactory.MakeOrder()).ToList();
        foreach (var order in orders)
            await repository.Create(order, connection, null!);

        // Act
        var result = await repository.GetAll(1, 10);

        // Assert
        result.Items.Should().BeInDescendingOrder(order => order.CreatedOn);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyPage_WhenNoOrders()
    {
        // Act
        var result = await repository.GetAll(1, 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
    
    [Fact]
    public async Task Save_ShouldSaveStatus_AndReturnId()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();;
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        await repository.Create(order, connection, null!);
        
        // Act
        order.Pay();
        var returnedId = await repository.Save(order, connection, null!);
        var sql = "SELECT  * FROM orders WHERE id = @id";
        var row = await connection.QueryFirstOrDefaultAsync<OrderDao>(sql, new { id = order.Id });

        //Assert
        Assert.Equal(order.Id, returnedId);
        row.ToDomain().Status.Should().Be(Status.Paid);
    }
    
    [Fact]
    public async Task Save_ShouldThrowException_WhenOrderDoesNotExist()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();;
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        
        // Act
        order.Pay();
        var action = async () => await repository.Save(order, connection, null!);
        
        // Assert 
        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Delete_ShouldDeleteOrder()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();;
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        var returnedId = await repository.Create(order, connection, null!);
        
        // Act
        await repository.Delete(order.Id, connection, null!);
        var sql = "SELECT  * FROM orders WHERE id = @id";
        var row = await connection.QueryFirstOrDefaultAsync<OrderDao>(sql, new { id = order.Id });
        
        // Assert
        row.ToDomain().Should().BeNull();
    }
    
    [Fact]
    public async Task Delete_ShouldThrowException_WhenOrderDoesNotExist()
    {
        // Arrange
        var order = EntityFactory.MakeOrder();;
        await using var connection = new NpgsqlConnection(postgresFixture.Container.GetConnectionString());
        
        // Act
        var action = async () => await repository.Delete(order.Id, connection, null!);
        
        // Assert 
        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
}