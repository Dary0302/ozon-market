using Core.Common.Kafka;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Common.Extensions;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddSingleton<PendingRequestRegistry>();
        services.AddSingleton(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));
        return services;
    }

    public static IServiceCollection AddKafkaRequestClient<TRequest, TResponse>(this IServiceCollection services)
        where TRequest : class, IHasCorrelationId
        where TResponse : class, IHasCorrelationId
    {
        services.AddSingleton<IKafkaRequestClient<TRequest, TResponse>, KafkaRequestClient<TRequest, TResponse>>();
        return services;
    }
}