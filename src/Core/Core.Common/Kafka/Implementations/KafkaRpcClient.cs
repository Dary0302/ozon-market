using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Implementations;

public class KafkaRpcClient : IKafkaRpcClient
{
    private readonly IKafkaProducer producer;
    private readonly PendingRequestRegistry registry;

    public KafkaRpcClient(
        IKafkaProducer producer,
        PendingRequestRegistry registry)
    {
        this.producer = producer;
        this.registry = registry;
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(
        string topic,
        TRequest request,
        CancellationToken ct = default,
        TimeSpan? timeout = null)
        where TRequest : class, IHasCorrelationId
        where TResponse : class, IHasCorrelationId
    {
        var tcs = new TaskCompletionSource<TResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        registry.Register(request.CorrelationId, tcs);

        try
        {
            await producer.ProduceAsync(topic, request, ct); 

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct); 
            cts.CancelAfter(timeout ?? TimeSpan.FromSeconds(30));

            await using var _ = cts.Token.Register(() =>
            {
                registry.Remove(request.CorrelationId);
                tcs.TrySetException(ct.IsCancellationRequested
                    ? new OperationCanceledException(ct) 
                    : new TimeoutException($"No response for {request.CorrelationId}"));
            });

            return await tcs.Task;
        }
        finally
        {
            registry.Remove(request.CorrelationId);
        }
    }
}