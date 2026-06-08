using FluentAssertions;
using StorageService.Domain;
using StorageService.Infrastructure.Implementations;
using Xunit;
using Core.Common.DbHelpers;
using Npgsql;
using Dapper;
using StorageService.Infrastructure.Models;
using StorageService.Tests.Helpers;

namespace StorageService.Tests.Infrastructure;

public class StorageRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture fixture;
    private readonly StorageRepository repository;

    public StorageRepositoryTests(PostgresFixture fixture)
    {
        this.fixture = fixture;
        var connectionFactory = new PostgresConnectionFactory(fixture.Container.GetConnectionString());
        repository = new StorageRepository(connectionFactory);
    }

    [Fact]
    public async Task Add_ValidStorage_ShouldSaveToDatabase()
    {
        var storage = Storage.Restore(Guid.NewGuid(), "Test Address 123", Guid.NewGuid());

        await repository.Add(storage, CancellationToken.None);
        
        await using var connection = new NpgsqlConnection(fixture.Container.GetConnectionString());
        var result = await connection.QueryFirstOrDefaultAsync<StorageDao>(
            "SELECT id AS Id, address AS Address, point_id AS PointId FROM storages WHERE id = @id",
            new { id = storage.Id });
        
        result.Should().NotBeNull();
        result.Id.Should().Be(storage.Id);
        result.Address.Should().Be(storage.Address);
        result.PointId.Should().Be(storage.PointId);
    }

    [Fact]
    public async Task Get_ExistingId_ShouldReturnStorage()
    {
        var storage = Storage.Restore(Guid.NewGuid(), "Main Street 42", Guid.NewGuid());
        
        await repository.Add(storage, CancellationToken.None);

        var result = await repository.Get(storage.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(storage.Id);
        result.Address.Should().Be(storage.Address);
        result.PointId.Should().Be(storage.PointId);
    }

    [Fact]
    public async Task Get_NonExistingId_ShouldReturnNull()
    {
        var nonExistingId = Guid.NewGuid();

        var result = await repository.Get(nonExistingId, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_ExistingStorage_ShouldUpdateFields()
    {
        var originalStorage = Storage.Restore(Guid.NewGuid(), "Original Address", Guid.NewGuid());
        
        await repository.Add(originalStorage, CancellationToken.None);
        
        var updatedStorage = Storage.Restore(originalStorage.Id, "Updated Address 999", Guid.NewGuid());

        await repository.Update(updatedStorage, CancellationToken.None);
        
        var result = await repository.Get(originalStorage.Id, CancellationToken.None);
        result.Should().NotBeNull();
        result.Address.Should().Be("Updated Address 999");
        result.PointId.Should().Be(updatedStorage.PointId);
    }

    [Fact]
    public async Task Delete_ExistingStorage_ShouldRemoveFromDatabase()
    {
        var storage = Storage.Restore(Guid.NewGuid(), "To Be Deleted", Guid.NewGuid());
        
        await repository.Add(storage, CancellationToken.None);
        
        await repository.Delete(storage.Id, CancellationToken.None);
        
        var result = await repository.Get(storage.Id, CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NonExistingId_ShouldNotThrowException()
    {
        var nonExistingId = Guid.NewGuid();

        var act = async () => await repository.Delete(nonExistingId, CancellationToken.None);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Add_DuplicateId_ShouldThrowException()
    {
        var storageId = Guid.NewGuid();
        var storage1 = Storage.Restore(storageId, "First Address", Guid.NewGuid());
        var storage2 = Storage.Restore(storageId, "Second Address", Guid.NewGuid());
        
        await repository.Add(storage1, CancellationToken.None);

        var act = async () => await repository.Add(storage2, CancellationToken.None);
        
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task MultipleOperations_ShouldWorkCorrectly()
    {
        var storage1 = Storage.Restore(Guid.NewGuid(), "Storage 1", Guid.NewGuid());
        var storage2 = Storage.Restore(Guid.NewGuid(), "Storage 2", Guid.NewGuid());
        var storage3 = Storage.Restore(Guid.NewGuid(), "Storage 3", Guid.NewGuid());

        await repository.Add(storage1, CancellationToken.None);
        await repository.Add(storage2, CancellationToken.None);
        await repository.Add(storage3, CancellationToken.None);
        
        var retrieved2 = await repository.Get(storage2.Id, CancellationToken.None);
        await repository.Delete(storage2.Id, CancellationToken.None);
        
        var deleted = await repository.Get(storage2.Id, CancellationToken.None);
        var stillExists1 = await repository.Get(storage1.Id, CancellationToken.None);
        var stillExists3 = await repository.Get(storage3.Id, CancellationToken.None);
        
        retrieved2.Should().NotBeNull();
        retrieved2.Id.Should().Be(storage2.Id);
        
        deleted.Should().BeNull();
        stillExists1.Should().NotBeNull();
        stillExists3.Should().NotBeNull();
    }
}