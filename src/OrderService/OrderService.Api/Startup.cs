using Core.Common.Extensions;
using OrderService.Infrastructure;
using OrderService.Application;

namespace OrderService.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddValidation();
        services.AddCorsPolicy();

        services.AddApplicationServices()
            .AddOpenApi(
                "OrderService",
                typeof(Startup));

        services.AddInfrastructureServices(configuration);
    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .UseRouting()
            .UseCorsPolicy()
            .UseOpenApi()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            }
        );
    }
}