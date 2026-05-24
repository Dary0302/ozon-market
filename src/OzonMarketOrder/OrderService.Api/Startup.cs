using OrderService.Infrastructure;
using OrderService.Api.Extensions;
using OrderService.Application;

namespace OrderService.Api;

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