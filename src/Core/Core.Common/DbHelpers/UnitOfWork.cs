using System.Data;
using Npgsql;

namespace Core.Common.DbHelpers.Interfaces;

public class UnitOfWork(IPostgresConnectionFactory connectionFactory) : IUnitOfWork
{
    public NpgsqlConnection? Connection { get; private set; }
    public NpgsqlTransaction? Transaction { get; private set; }
    
    public NpgsqlConnection CurrentConnection =>
        Connection ?? throw new InvalidOperationException("No active connection");

    public NpgsqlTransaction CurrentTransaction =>
        Transaction ?? throw new InvalidOperationException("No active transaction");
    
    private bool HasActiveTransaction =>
        Transaction is not null;
    
    private bool HasActiveConnection =>
        Connection is not null;
    
    public async Task BeginTransactionAsync()
    {
        if (HasActiveTransaction)
            throw new InvalidOperationException("Transaction is already started");
        if (HasActiveConnection)
            throw new InvalidOperationException("Connection is already started");
        Connection = connectionFactory.GetConnection();
        await Connection.OpenAsync();
        Transaction = await Connection.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await CurrentTransaction.CommitAsync();
        await CleanupAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await CurrentTransaction.RollbackAsync();
        await CleanupAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (HasActiveTransaction)
            await Transaction.DisposeAsync();

        if (HasActiveConnection)
            await Connection.DisposeAsync();
    }
    
    public async Task ExecuteInTransaction(Func<Task> action)
    {
        await BeginTransactionAsync();

        try
        {
            await action();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        await CommitTransactionAsync();
    }
    
    private async Task CleanupAsync()
    {
        if (Transaction is not null)
        {
            await Transaction.DisposeAsync();
            Transaction = null;
        }

        if (Connection is not null)
        {
            await Connection.DisposeAsync();
            Connection = null;
        }
    }
}