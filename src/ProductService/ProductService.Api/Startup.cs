using Core.Common.Extensions;
using Core.Common.Extensions.Validation;
using ProductService.Application;
using ProductService.Infrastructure.Configurations;
namespace ProductService.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddInfrastructureServices(configuration)
            .AddApplicationServices()
            .AddOpenApi("ProductService", typeof(ApplicationConfiguration))
            .AddValidation()
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
            });
    }
}