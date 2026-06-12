using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Implementations;

public class KafkaRequestClient<TRequest, TResponse> 
    : IKafkaRequestClient<TRequest, TResponse>
    where TRequest  : class, IHasCorrelationId
    where TResponse : class, IHasCorrelationId
{
    private readonly IKafkaProducer<TRequest> producer;
    private readonly PendingRequestRegistry registry;
    private readonly string requestTopic;
    
    public KafkaRequestClient(IKafkaProducer<TRequest> producer, PendingRequestRegistry registry, string requestTopic)
    {
        this.producer = producer;
        this.registry = registry;
        this.requestTopic = requestTopic;
    }

    public async Task<TResponse> RequestAsync(TRequest request, TimeSpan? timeout = null)
    {
        var tcs = new TaskCompletionSource<TResponse>();
        registry.Register(request.CorrelationId, tcs);

        await producer.ProduceAsync(requestTopic, request);

        using var cts = new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(30));
        cts.Token.Register(() =>
        {
            registry.Remove(request.CorrelationId);
            tcs.TrySetException(new TimeoutException($"No response for {request.CorrelationId}"));
        });

        return await tcs.Task;
    }
}