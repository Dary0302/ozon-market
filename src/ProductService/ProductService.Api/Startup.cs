using Core.Common.Extensions;
using Core.Common.Extensions.Validation;
using Core.Common.HttpLogic;
using ProductService.Api.Validators;
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
            .AddCorsPolicy()
            .AddExceptionHandler<ExceptionHandler>()
            .AddProblemDetails()
            .AddOpenApi("ProductService", typeof(ApplicationConfiguration))
            .AddValidation<IValidatorMarker>()
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
            });
    }
}
