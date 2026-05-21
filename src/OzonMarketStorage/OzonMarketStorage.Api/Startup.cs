using OzonMarketStorage.Infrastructure.Helpers;

namespace OzonMarketStorage.Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var connectionString = _configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new NullReferenceException("DefaultConnection");
        services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}