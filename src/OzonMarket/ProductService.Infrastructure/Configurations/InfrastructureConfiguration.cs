using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Helpers;
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
    
        return services;
    }
}