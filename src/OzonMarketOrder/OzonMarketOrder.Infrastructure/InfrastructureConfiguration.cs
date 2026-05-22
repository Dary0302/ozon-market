using Core.Common.DbHelpers;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzonMarketOrder.Application.Interfaces;
using OzonMarketOrder.Infrastructure.Implementations;

namespace OzonMarketOrder.Infrastructure;

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
    
        return services;
    }
}