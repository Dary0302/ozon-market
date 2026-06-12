using System.Collections.Concurrent;

namespace Core.Common.Kafka.Implementations;

public class PendingRequestRegistry
{
    private readonly ConcurrentDictionary<Guid, object> _pending = new();

    public void Register<T>(Guid correlationId, TaskCompletionSource<T> tcs)
        => _pending[correlationId] = tcs;

    public void Complete<T>(Guid correlationId, T result)
    {
        if (_pending.TryRemove(correlationId, out var obj) 
            && obj is TaskCompletionSource<T> tcs)
            tcs.TrySetResult(result);
    }

    public void Remove(Guid correlationId) => _pending.TryRemove(correlationId, out _);
}