using StorageService.Api.Extensions;
using StorageService.Application;
using StorageService.Application.Configurations;
using StorageService.Infrastructure.Configurations;

namespace StorageService.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddApplicationServices()
            .AddOpenApi();

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