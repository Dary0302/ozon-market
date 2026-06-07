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

public class PvzRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture fixture;
    private readonly PvzRepository repository;

    public PvzRepositoryTests(PostgresFixture fixture)
    {
        this.fixture = fixture;
        var connectionFactory = new PostgresConnectionFactory(fixture.Container.GetConnectionString());
        repository = new PvzRepository(connectionFactory);
        var connection = new NpgsqlConnection(fixture.Container.GetConnectionString());
        
        // Очищаем таблицу перед каждым тестом
        connection.Open();
        connection.Execute("TRUNCATE TABLE pvz CASCADE");
        connection.Close();
    }

    [Fact]
    public async Task Add_ValidPvz_ShouldSaveToDatabase()
    {
        var pvz = Pvz.Restore(Guid.NewGuid(), "Test Pvz Address 123", Guid.NewGuid());

        await repository.Add(pvz, CancellationToken.None);
        
        await using var connection = new NpgsqlConnection(fixture.Container.GetConnectionString());
        var result = await connection.QueryFirstOrDefaultAsync<PvzDao>(
            "SELECT id, address, point_id FROM pvz WHERE id = @id",
            new { id = pvz.Id });
        
        result.Should().NotBeNull();
        result.Id.Should().Be(pvz.Id);
        result.Address.Should().Be(pvz.Address);
        result.PointId.Should().Be(pvz.PointId);
    }

    [Fact]
    public async Task Get_ExistingId_ShouldReturnPvz()
    {
        var pvz = Pvz.Restore(Guid.NewGuid(), "Main Street Pvz 42", Guid.NewGuid());
        
        await repository.Add(pvz, CancellationToken.None);

        var result = await repository.Get(pvz.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(pvz.Id);
        result.Address.Should().Be(pvz.Address);
        result.PointId.Should().Be(pvz.PointId);
    }

    [Fact]
    public async Task Get_NonExistingId_ShouldReturnNull()
    {
        var nonExistingId = Guid.NewGuid();

        var result = await repository.Get(nonExistingId, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_WhenPvzExist_ShouldReturnAllPvz()
    {
        var pvz1 = Pvz.Restore(Guid.NewGuid(), "Pvz 1", Guid.NewGuid());
        var pvz2 = Pvz.Restore(Guid.NewGuid(), "Pvz 2", Guid.NewGuid());
        var pvz3 = Pvz.Restore(Guid.NewGuid(), "Pvz 3", Guid.NewGuid());

        await repository.Add(pvz1, CancellationToken.None);
        await repository.Add(pvz2, CancellationToken.None);
        await repository.Add(pvz3, CancellationToken.None);

        var result = await repository.GetAll(CancellationToken.None);

        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Id == pvz1.Id);
        result.Should().Contain(p => p.Id == pvz2.Id);
        result.Should().Contain(p => p.Id == pvz3.Id);
    }

    [Fact]
    public async Task GetAll_WhenNoPvzExist_ShouldReturnEmptyList()
    {
        var result = await repository.GetAll(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_ExistingPvz_ShouldUpdateFields()
    {
        var originalPvz = Pvz.Restore(Guid.NewGuid(), "Original Pvz Address", Guid.NewGuid());
        
        await repository.Add(originalPvz, CancellationToken.None);
        
        var updatedPvz = Pvz.Restore(originalPvz.Id, "Updated Pvz Address 999", Guid.NewGuid());

        await repository.Update(updatedPvz, CancellationToken.None);
        
        var result = await repository.Get(originalPvz.Id, CancellationToken.None);
        result.Should().NotBeNull();
        result.Address.Should().Be("Updated Pvz Address 999");
        result.PointId.Should().Be(updatedPvz.PointId);
    }

    [Fact]
    public async Task Update_NonExistingPvz_ShouldNotThrowException()
    {
        var nonExistingPvz = Pvz.Restore(Guid.NewGuid(), "Non Existing", Guid.NewGuid());

        Func<Task> act = async () => await repository.Update(nonExistingPvz, CancellationToken.None);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Delete_ExistingPvz_ShouldRemoveFromDatabase()
    {
        var pvz = Pvz.Restore(Guid.NewGuid(), "To Be Deleted Pvz", Guid.NewGuid());
        
        await repository.Add(pvz, CancellationToken.None);
        
        await repository.Delete(pvz.Id, CancellationToken.None);
        
        var result = await repository.Get(pvz.Id, CancellationToken.None);
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
        var pvzId = Guid.NewGuid();
        var pvz1 = Pvz.Restore(pvzId, "First Pvz", Guid.NewGuid());
        var pvz2 = Pvz.Restore(pvzId, "Second Pvz", Guid.NewGuid());
        
        await repository.Add(pvz1, CancellationToken.None);

        Func<Task> act = async () => await repository.Add(pvz2, CancellationToken.None);
        
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task MultipleOperations_ShouldWorkCorrectly()
    {
        var pvz1 = Pvz.Restore(Guid.NewGuid(), "Pvz Alpha", Guid.NewGuid());
        var pvz2 = Pvz.Restore(Guid.NewGuid(), "Pvz Beta", Guid.NewGuid());
        var pvz3 = Pvz.Restore(Guid.NewGuid(), "Pvz Gamma", Guid.NewGuid());

        await repository.Add(pvz1, CancellationToken.None);
        await repository.Add(pvz2, CancellationToken.None);
        await repository.Add(pvz3, CancellationToken.None);
        
        var allBeforeDelete = await repository.GetAll(CancellationToken.None);
        allBeforeDelete.Should().HaveCount(3);
        
        var retrieved2 = await repository.Get(pvz2.Id, CancellationToken.None);
        await repository.Delete(pvz2.Id, CancellationToken.None);
        
        var allAfterDelete = await repository.GetAll(CancellationToken.None);
        var deleted = await repository.Get(pvz2.Id, CancellationToken.None);
        var stillExists1 = await repository.Get(pvz1.Id, CancellationToken.None);
        var stillExists3 = await repository.Get(pvz3.Id, CancellationToken.None);
        
        retrieved2.Should().NotBeNull();
        retrieved2.Id.Should().Be(pvz2.Id);
        
        deleted.Should().BeNull();
        stillExists1.Should().NotBeNull();
        stillExists3.Should().NotBeNull();
        allAfterDelete.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCorrectOrder_NoSpecificOrderRequired()
    {
        var pvzA = Pvz.Restore(Guid.NewGuid(), "A Pvz", Guid.NewGuid());
        var pvzB = Pvz.Restore(Guid.NewGuid(), "B Pvz", Guid.NewGuid());
        var pvzC = Pvz.Restore(Guid.NewGuid(), "C Pvz", Guid.NewGuid());

        await repository.Add(pvzC, CancellationToken.None);
        await repository.Add(pvzA, CancellationToken.None);
        await repository.Add(pvzB, CancellationToken.None);

        var result = await repository.GetAll(CancellationToken.None);

        result.Should().Contain(new[] { pvzA, pvzB, pvzC });
        result.Should().HaveCount(3);
    }
}