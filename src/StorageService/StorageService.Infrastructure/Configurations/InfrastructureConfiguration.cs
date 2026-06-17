using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using Core.Common.Extensions;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Kafka.Consumers;
using StorageService.Infrastructure.Implementations;

namespace StorageService.Infrastructure.Configurations;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new NullReferenceException("No database connection string found.");

        services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));

        services.AddSingleton<IStorageRepository, StorageRepository>();
        services.AddSingleton<IStoragePointRepository, StoragePointRepository>();
        services.AddSingleton<IPvzRepository, PvzRepository>();
        services.AddSingleton<IPvzPointRepository, PvzPointRepository>();
        services.AddSingleton<IStoredProductRepository, StoredProductRepository>();
        
        services.AddKafkaServices(configuration);
    
        return services;
    }
    
    private static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafkaCore(configuration);

        services.AddHostedService<CheckStockRequestConsumer>();
        services.AddHostedService<DeliveryDateRequestConsumer>();
        services.AddHostedService<ProductStorageRequestConsumer>();
        services.AddHostedService<ReduceCountOfProductsCommandConsumer>();
        services.AddHostedService<ReturnProductsToStorageCommandConsumer>();

        return services;
    }
}