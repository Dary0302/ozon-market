using Core.Common;
using Dapper;
using Core.Common.DbHelpers;
using OzonMarketOrder.Api.Extensions;
using OzonMarketOrder.Application;
using OzonMarketOrder.Application.Interfaces;
using OzonMarketOrder.Infrastructure.Implementations;

namespace OzonMarketOrder.Api;

public class Startup(IConfiguration configuration, IHostBuilder hostBuilder)
{
    public void ConfigureServices(IServiceCollection services)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        services.AddControllers();

        services.AddApplicationServices()
            .AddOpenApi();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new NullReferenceException("No database connection string found.");;
        
        services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));
        services.AddSingleton<IOrderRepository, OrderRepository>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            }
        );
    }
}