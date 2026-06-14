using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using Core.Common.Extensions;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Kafka.Consumers;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Infrastructure.Configurations;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new NullReferenceException("No database connection string found.");;

        services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));
        services.AddSingleton<IPriceRepository, PriceRepository>();
        services.AddSingleton<IProductRepository, ProductRepository>();
        
        services.AddKafkaServices(configuration);
    
        return services;
    }
    
    private static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafkaCore(configuration);
        
        services.AddHostedService<GetProductsPriceRequestConsumer>();

        return services;
    }
}