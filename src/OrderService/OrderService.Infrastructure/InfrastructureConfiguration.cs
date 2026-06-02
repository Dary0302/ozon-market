using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
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
    
        return services;
    }
}