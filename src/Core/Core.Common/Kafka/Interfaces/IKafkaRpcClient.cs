namespace Core.Common.Kafka.Interfaces;

public interface IKafkaRpcClient
{
    Task<TResponse> RequestAsync<TRequest, TResponse>(
        string topic,
        TRequest request,
        TimeSpan? timeout = null)
        where TRequest : class, IHasCorrelationId
        where TResponse : class, IHasCorrelationId;
}