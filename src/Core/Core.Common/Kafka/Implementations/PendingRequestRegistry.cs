using System.Collections.Concurrent;

namespace Core.Common.Kafka.Implementations;

public class PendingRequestRegistry
{
    private readonly ConcurrentDictionary<Guid, object> pending = new();

    public void Register<T>(Guid correlationId, TaskCompletionSource<T> tcs)
        => pending[correlationId] = tcs;

    public void Complete<T>(Guid correlationId, T result)
    {
        if (pending.TryRemove(correlationId, out var obj) 
            && obj is TaskCompletionSource<T> tcs)
            tcs.TrySetResult(result);
    }

    public void Remove(Guid correlationId) => pending.TryRemove(correlationId, out _);
}