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

public class PvzPointRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture fixture;
    private readonly PvzPointRepository repository;
    private readonly PvzRepository pvzRepository;
    private readonly string connectionString;

    public PvzPointRepositoryTests(PostgresFixture fixture)
    {
        this.fixture = fixture;
        this.connectionString = fixture.Container.GetConnectionString();
        var connectionFactory = new PostgresConnectionFactory(connectionString);
        repository = new PvzPointRepository(connectionFactory);
        pvzRepository = new PvzRepository(connectionFactory);
        
        // Очищаем таблицы перед каждым тестом (сначала points, потом pvz из-за внешнего ключа)
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.Execute("TRUNCATE TABLE pvz_points CASCADE");
        connection.Execute("TRUNCATE TABLE pvz CASCADE");
    }

    private async Task<Pvz> CreateTestPvz()
    {
        var pvz = Pvz.Restore(Guid.NewGuid(), "Test Pvz Address", Guid.NewGuid());
        await pvzRepository.Add(pvz, CancellationToken.None);
        return pvz;
    }

    [Fact]
    public async Task Add_ValidPvzPoint_ShouldSaveToDatabase()
    {
        var pvz = await CreateTestPvz();
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);

        await repository.Add(pvzPoint, CancellationToken.None);
        
        using var connection = new NpgsqlConnection(connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<PvzPointDao>(
            "SELECT id, pvz_id, longitude, latitude FROM pvz_points WHERE id = @pointId",
            new { pointId = pvzPoint.Id });
        
        result.Should().NotBeNull();
        result.Id.Should().Be(pvzPoint.Id);
        result.PvzId.Should().Be(pvzPoint.PvzId);
        result.Longitude.Should().Be(pvzPoint.Longitude);
        result.Latitude.Should().Be(pvzPoint.Latitude);
    }

    [Fact]
    public async Task Get_ExistingId_ShouldReturnPvzPoint()
    {
        var pvz = await CreateTestPvz();
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        
        await repository.Add(pvzPoint, CancellationToken.None);

        var result = await repository.Get(pvzPoint.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(pvzPoint.Id);
        result.PvzId.Should().Be(pvzPoint.PvzId);
        result.Longitude.Should().Be(pvzPoint.Longitude);
        result.Latitude.Should().Be(pvzPoint.Latitude);
    }

    [Fact]
    public async Task Get_NonExistingId_ShouldReturnNull()
    {
        var nonExistingId = Guid.NewGuid();

        var result = await repository.Get(nonExistingId, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_ExistingPvzPoint_ShouldUpdateFields()
    {
        var pvz = await CreateTestPvz();
        var anotherPvz = await CreateTestPvz();
        var originalPvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        
        await repository.Add(originalPvzPoint, CancellationToken.None);
        
        var updatedPvzPoint = PvzPoint.Restore(
            originalPvzPoint.Id, 
            anotherPvz.Id, 
            59.934280, 
            30.335099);

        await repository.Update(updatedPvzPoint, CancellationToken.None);
        
        var result = await repository.Get(originalPvzPoint.Id, CancellationToken.None);
        result.Should().NotBeNull();
        result.PvzId.Should().Be(updatedPvzPoint.PvzId);
        result.Longitude.Should().Be(updatedPvzPoint.Longitude);
        result.Latitude.Should().Be(updatedPvzPoint.Latitude);
    }

    [Fact]
    public async Task Update_NonExistingPvzPoint_ShouldNotThrowException()
    {
        var nonExistingPvzPoint = PvzPoint.Restore(Guid.NewGuid(), Guid.NewGuid(), 55.751244, 37.618423);

        Func<Task> act = async () => await repository.Update(nonExistingPvzPoint, CancellationToken.None);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Delete_ExistingId_ShouldRemoveFromDatabase()
    {
        var pvz = await CreateTestPvz();
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        
        await repository.Add(pvzPoint, CancellationToken.None);
        
        await repository.Delete(pvzPoint.Id, CancellationToken.None);
        
        var result = await repository.Get(pvzPoint.Id, CancellationToken.None);
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
        var pvz = await CreateTestPvz();
        var pointId = Guid.NewGuid();
        var pvzPoint1 = PvzPoint.Restore(pointId, pvz.Id, 55.751244, 37.618423);
        var pvzPoint2 = PvzPoint.Restore(pointId, pvz.Id, 59.934280, 30.335099);
        
        await repository.Add(pvzPoint1, CancellationToken.None);

        Func<Task> act = async () => await repository.Add(pvzPoint2, CancellationToken.None);
        
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task MultipleOperations_ShouldWorkCorrectly()
    {
        var pvz = await CreateTestPvz();
        var point1 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        var point2 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 59.934280, 30.335099);
        var point3 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 54.734680, 55.957855);

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
    public async Task Add_MultiplePointsForSamePvz_ShouldWorkCorrectly()
    {
        var pvz = await CreateTestPvz();
        var point1 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        var point2 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 59.934280, 30.335099);
        var point3 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 54.734680, 55.957855);

        await repository.Add(point1, CancellationToken.None);
        await repository.Add(point2, CancellationToken.None);
        await repository.Add(point3, CancellationToken.None);

        using var connection = new NpgsqlConnection(connectionString);
        var results = await connection.QueryAsync<PvzPointDao>(
            "SELECT id, pvz_id, longitude, latitude FROM pvz_points WHERE pvz_id = @pvzId",
            new { pvzId = pvz.Id });
        
        results.Should().HaveCount(3);
        results.Should().Contain(p => p.Id == point1.Id);
        results.Should().Contain(p => p.Id == point2.Id);
        results.Should().Contain(p => p.Id == point3.Id);
    }

    [Fact]
    public async Task Update_ShouldOnlyAffectSpecifiedPoint()
    {
        var pvz = await CreateTestPvz();
        var point1 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 55.751244, 37.618423);
        var point2 = PvzPoint.Restore(Guid.NewGuid(), pvz.Id, 59.934280, 30.335099);
        
        await repository.Add(point1, CancellationToken.None);
        await repository.Add(point2, CancellationToken.None);
        
        var updatedPoint1 = PvzPoint.Restore(point1.Id, pvz.Id, 54.734680, 55.957855);
        await repository.Update(updatedPoint1, CancellationToken.None);
        
        var result1 = await repository.Get(point1.Id, CancellationToken.None);
        var result2 = await repository.Get(point2.Id, CancellationToken.None);
        
        result1.Longitude.Should().Be(54.734680);
        result1.Latitude.Should().Be(55.957855);
        
        result2.Longitude.Should().Be(59.934280);
        result2.Latitude.Should().Be(30.335099);
    }

    [Fact]
    public async Task Add_PointWithNonExistentPvz_ShouldThrowForeignKeyException()
    {
        var nonExistentPvzId = Guid.NewGuid();
        var pvzPoint = PvzPoint.Restore(Guid.NewGuid(), nonExistentPvzId, 55.751244, 37.618423);

        Func<Task> act = async () => await repository.Add(pvzPoint, CancellationToken.None);
        
        await act.Should().ThrowAsync<Exception>();
    }
}