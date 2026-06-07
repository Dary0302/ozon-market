using Core.Common.DbHelpers;
using Dapper;
using FluentAssertions;
using Npgsql;
using StorageService.Domain;
using StorageService.Infrastructure.Implementations;
using StorageService.Infrastructure.Models;
using StorageService.Tests.Helpers;
using Xunit;

namespace StorageService.Tests.Infrastructure;

public class StoragePointRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture fixture;
    private readonly StoragePointRepository repository;
    private readonly StorageRepository storageRepository;
    private readonly string connectionString;

    public StoragePointRepositoryTests(PostgresFixture fixture)
    {
        this.fixture = fixture;
        this.connectionString = fixture.Container.GetConnectionString();
        var connectionFactory = new PostgresConnectionFactory(connectionString);
        repository = new StoragePointRepository(connectionFactory);
        storageRepository = new StorageRepository(connectionFactory);
        
        // Очищаем таблицы перед каждым тестом (сначала points, потом storage из-за внешнего ключа)
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.Execute("TRUNCATE TABLE storage_points CASCADE");
        connection.Execute("TRUNCATE TABLE storages CASCADE");
    }

    private async Task<Storage> CreateTestStorage()
    {
        var storage = Storage.Restore(Guid.NewGuid(), "Test Storage Address", Guid.NewGuid());
        await storageRepository.Add(storage, CancellationToken.None);
        return storage;
    }

    [Fact]
    public async Task Add_ValidStoragePoint_ShouldSaveToDatabase()
    {
        var storage = await CreateTestStorage();
        var storagePoint = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);

        await repository.Add(storagePoint, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<StoragePointDao>(
            "SELECT id, storage_id, longitude, latitude FROM storage_points WHERE id = @id",
            new { id = storagePoint.Id });
        
        result.Should().NotBeNull();
        result.Id.Should().Be(storagePoint.Id);
        result.StorageId.Should().Be(storagePoint.StorageId);
        result.Longitude.Should().Be(storagePoint.Longitude);
        result.Latitude.Should().Be(storagePoint.Latitude);
    }

    [Fact]
    public async Task Get_ExistingId_ShouldReturnStoragePoint()
    {
        var storage = await CreateTestStorage();
        var storagePoint = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        
        await repository.Add(storagePoint, CancellationToken.None);

        var result = await repository.Get(storagePoint.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(storagePoint.Id);
        result.StorageId.Should().Be(storagePoint.StorageId);
        result.Longitude.Should().Be(storagePoint.Longitude);
        result.Latitude.Should().Be(storagePoint.Latitude);
    }

    [Fact]
    public async Task Get_NonExistingId_ShouldReturnNull()
    {
        var nonExistingId = Guid.NewGuid();

        var result = await repository.Get(nonExistingId, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStoragePoints_WithNonExistingIds_ShouldReturnEmptyList()
    {
        var nonExistingIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var result = await repository.GetStoragePoints(nonExistingIds, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStoragePoints_WithEmptyList_ShouldReturnEmptyList()
    {
        var emptyIds = new List<Guid>();

        var result = await repository.GetStoragePoints(emptyIds, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStoragePoints_WithMixedIds_ShouldReturnOnlyExisting()
    {
        var storage = await CreateTestStorage();
        var existingPoint = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        await repository.Add(existingPoint, CancellationToken.None);

        var storageIds = new List<Guid> { existingPoint.StorageId, Guid.NewGuid(), Guid.NewGuid() };
        var result = await repository.GetStoragePoints(storageIds, CancellationToken.None);

        result.Should().HaveCount(1);
        result.Should().Contain(p => p.Id == existingPoint.Id);
    }

    [Fact]
    public async Task Update_ExistingStoragePoint_ShouldUpdateFields()
    {
        var storage = await CreateTestStorage();
        var anotherStorage = await CreateTestStorage();
        var originalPoint = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        
        await repository.Add(originalPoint, CancellationToken.None);
        
        var updatedPoint = StoragePoint.Restore(
            originalPoint.Id, 
            anotherStorage.Id, 
            59.934280, 
            30.335099);

        await repository.Update(updatedPoint, CancellationToken.None);
        
        var result = await repository.Get(originalPoint.Id, CancellationToken.None);
        result.Should().NotBeNull();
        result.StorageId.Should().Be(updatedPoint.StorageId);
        result.Longitude.Should().Be(updatedPoint.Longitude);
        result.Latitude.Should().Be(updatedPoint.Latitude);
    }

    [Fact]
    public async Task Update_NonExistingStoragePoint_ShouldNotThrowException()
    {
        var nonExistingPoint = StoragePoint.Restore(Guid.NewGuid(), Guid.NewGuid(), 55.751244, 37.618423);

        Func<Task> act = async () => await repository.Update(nonExistingPoint, CancellationToken.None);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Delete_ExistingId_ShouldRemoveFromDatabase()
    {
        var storage = await CreateTestStorage();
        var storagePoint = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        
        await repository.Add(storagePoint, CancellationToken.None);
        
        await repository.Delete(storagePoint.Id, CancellationToken.None);
        
        var result = await repository.Get(storagePoint.Id, CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NonExistingId_ShouldNotThrowException()
    {
        var nonExistingId = Guid.NewGuid();

        Func<Task> act = async () => await repository.Delete(nonExistingId, CancellationToken.None);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Add_DuplicateId_ShouldThrowException()
    {
        var storage = await CreateTestStorage();
        var pointId = Guid.NewGuid();
        var point1 = StoragePoint.Restore(pointId, storage.Id, 55.751244, 37.618423);
        var point2 = StoragePoint.Restore(pointId, storage.Id, 59.934280, 30.335099);
        
        await repository.Add(point1, CancellationToken.None);

        Func<Task> act = async () => await repository.Add(point2, CancellationToken.None);
        
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task MultipleOperations_ShouldWorkCorrectly()
    {
        var storage = await CreateTestStorage();
        var point1 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        var point2 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 59.934280, 30.335099);
        var point3 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 54.734680, 55.957855);

        await repository.Add(point1, CancellationToken.None);
        await repository.Add(point2, CancellationToken.None);
        await repository.Add(point3, CancellationToken.None);
        
        var retrieved2 = await repository.Get(point2.Id, CancellationToken.None);
        await repository.Delete(point2.Id, CancellationToken.None);
        
        var deleted = await repository.Get(point2.Id, CancellationToken.None);
        var stillExists1 = await repository.Get(point1.Id, CancellationToken.None);
        var stillExists3 = await repository.Get(point3.Id, CancellationToken.None);
        
        retrieved2.Should().NotBeNull();
        retrieved2.Id.Should().Be(point2.Id);
        
        deleted.Should().BeNull();
        stillExists1.Should().NotBeNull();
        stillExists3.Should().NotBeNull();
    }

    [Fact]
    public async Task Add_MultiplePointsForSameStorage_ShouldWorkCorrectly()
    {
        var storage = await CreateTestStorage();
        var point1 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        var point2 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 59.934280, 30.335099);
        var point3 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 54.734680, 55.957855);

        await repository.Add(point1, CancellationToken.None);
        await repository.Add(point2, CancellationToken.None);
        await repository.Add(point3, CancellationToken.None);

        using var connection = new NpgsqlConnection(connectionString);
        var results = await connection.QueryAsync<StoragePointDao>(
            "SELECT id, storage_id, longitude, latitude FROM storage_points WHERE storage_id = @storageId",
            new { storageId = storage.Id });
        
        results.Should().HaveCount(3);
        results.Should().Contain(p => p.Id == point1.Id);
        results.Should().Contain(p => p.Id == point2.Id);
        results.Should().Contain(p => p.Id == point3.Id);
    }

    [Fact]
    public async Task Update_ShouldOnlyAffectSpecifiedPoint()
    {
        var storage = await CreateTestStorage();
        var point1 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 55.751244, 37.618423);
        var point2 = StoragePoint.Restore(Guid.NewGuid(), storage.Id, 59.934280, 30.335099);
        
        await repository.Add(point1, CancellationToken.None);
        await repository.Add(point2, CancellationToken.None);
        
        var updatedPoint1 = StoragePoint.Restore(point1.Id, storage.Id, 54.734680, 55.957855);
        await repository.Update(updatedPoint1, CancellationToken.None);
        
        var result1 = await repository.Get(point1.Id, CancellationToken.None);
        var result2 = await repository.Get(point2.Id, CancellationToken.None);
        
        result1.Longitude.Should().Be(54.734680);
        result1.Latitude.Should().Be(55.957855);
        
        result2.Longitude.Should().Be(59.934280);
        result2.Latitude.Should().Be(30.335099);
    }

    [Fact]
    public async Task Add_PointWithNonExistentStorage_ShouldThrowForeignKeyException()
    {
        var nonExistentStorageId = Guid.NewGuid();
        var storagePoint = StoragePoint.Restore(Guid.NewGuid(), nonExistentStorageId, 55.751244, 37.618423);

        Func<Task> act = async () => await repository.Add(storagePoint, CancellationToken.None);
        
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetStoragePoints_WithNullCollection_ShouldReturnEmptyList()
    {
        var result = await repository.GetStoragePoints(null, CancellationToken.None);

        result.Should().BeEmpty();
    }
}