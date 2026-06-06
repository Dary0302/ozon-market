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

        services.AddApplicationServices()
            .AddOpenApi(
                "OrderService",
                typeof(OrderService.Api.Startup));

        services.AddInfrastructureServices(configuration);
    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .UseRouting()
            .UseOpenApi()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            }
        );
    }
}