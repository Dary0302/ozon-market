using System.Data;
using Npgsql;

namespace Core.Common.DbHelpers.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    NpgsqlConnection Connection { get; }
    NpgsqlTransaction Transaction { get; }
    
    NpgsqlConnection CurrentConnection { get; }
    NpgsqlTransaction CurrentTransaction { get; }
    
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task ExecuteInTransaction(Func<Task> action);
}