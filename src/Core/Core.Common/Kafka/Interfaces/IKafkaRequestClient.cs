namespace Core.Common.Kafka.Interfaces;

public interface IKafkaRequestClient<TRequest, TResponse>
    where TRequest : class, IHasCorrelationId
    where TResponse : class, IHasCorrelationId
{
    Task<TResponse> RequestAsync(TRequest request, TimeSpan? timeout = null);
}