using Core.Common.Extensions;
using Core.Common.Extensions.Validation;
using Core.Common.HttpLogic;
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
            .AddCorsPolicy()
            .AddExceptionHandler<ExceptionHandler>()
            .AddProblemDetails()
            .AddOpenApi("StorageService", typeof(ApplicationConfiguration))
            .AddValidation<IValidationMarker>()
            .AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .UseExceptionHandler()
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