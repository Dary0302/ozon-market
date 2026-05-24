using ProductService.Api.Extensions;
using ProductService.Application;
using ProductService.Infrastructure.Configurations;

namespace ProductService.Api;

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