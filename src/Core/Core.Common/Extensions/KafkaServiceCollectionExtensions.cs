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
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<IKafkaRpcClient, KafkaRpcClient>();
        return services;
    }
}