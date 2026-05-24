using System.Text.Json;
using System.Text.Json.Serialization;
using OzonMarketStorage.Infrastructure.Helpers;
using Core.Common;
using OzonMarketStorage.Api.Extensions;
using OzonMarketStorage.Application;

namespace OzonMarketStorage.Api;

public class Startup
{
    private readonly IConfiguration configuration;
    private readonly IHostBuilder hostBuilder;
    
    public Startup(IConfiguration configuration,  IHostBuilder hostBuilder)
    {
        this.configuration = configuration;
        this.hostBuilder = hostBuilder;
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor()
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        
        services.AddCore(hostBuilder)
            .AddApplicationServices()
            .AddOpenApi();

        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new NullReferenceException("DefaultConnection");
        services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        
        app.UseExceptionHandler();
        app.UseOpenApi();
        app.UseHttpsRedirection();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}