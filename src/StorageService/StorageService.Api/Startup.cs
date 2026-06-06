using StorageService.Api.Extensions;
using StorageService.Application;
using StorageService.Application.Configurations;
using StorageService.Infrastructure.Configurations;

namespace StorageService.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddInfrastructureServices(configuration)
            .AddApplicationServices()
            .AddOpenApi()
            .AddControllers();
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