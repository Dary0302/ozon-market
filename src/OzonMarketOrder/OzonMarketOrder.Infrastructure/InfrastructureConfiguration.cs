using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace OzonMarketOrder.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        
        dataSourceBuilder.EnableDynamicJson(); 
    
        var dataSource = dataSourceBuilder.Build();
        
        services.AddDbContext<DbContext>(options =>
            options
                .UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention());

        // TODO: добавить репозитории
        //services
            //.AddScoped<IAnimalRepository, AnimalRepository>()
    
        return services;
    }
}