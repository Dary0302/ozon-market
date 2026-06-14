using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using Core.Common.Extensions;
using Core.Common.Kafka.Contracts.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Application.Kafka.Consumers;
using OrderService.Application.Kafka.Services;
using OrderService.Infrastructure.Implementations;

namespace OrderService.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new NullReferenceException("No database connection string found.");;

        services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IOrderItemRepository, OrderItemRepository>();
        services.AddSingleton<IOrderInfoRepository, OrderInfoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddKafkaServices(configuration);
    
        return services;
    }
    
    private static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafkaCore(configuration);

        services.AddScoped<IProductService, KafkaProductService>();
        services.AddScoped<IStorageService, KafkaStorageService>();

        services.AddHostedService<CheckStockResponseConsumer>();
        services.AddHostedService<CalculateAmountResponseConsumer>();
        services.AddHostedService<DeliveryDateResponseConsumer>();
        services.AddHostedService<GetOrderSorageRecordsConsumer>();
        services.AddHostedService<GetProductsPriceResponseConsumer>();

        return services;
    }
}