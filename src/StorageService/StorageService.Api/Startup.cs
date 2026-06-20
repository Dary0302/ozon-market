using Core.Common.Extensions;
using Core.Common.Extensions.Validation;
using StorageService.Api.Validators;
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
            .AddOpenApi("StorageService", typeof(ApplicationConfiguration))
            .AddValidation<IValidationMarker>()
            .AddControllers();

        services.AddCorsPolicy();
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