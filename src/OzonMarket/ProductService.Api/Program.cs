using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Common;
using Core.Common.Migrations;
using ProductService.Api.Extensions;
using ProductService.Application;
using ProductService.Infrastructure.Helpers;
using ProductService.Infrastructure.Migrations;

namespace ProductService.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddHttpContextAccessor()
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        builder.Services
            .AddCore(builder.Host)
            .AddApplicationServices()
            .AddOpenApi();

        builder.Services.AddExceptionHandler<ExceptionHandler>();
        builder.Services.AddProblemDetails(); 
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddSingleton<IPostgresConnectionFactory>(new PostgresConnectionFactory(connectionString));
        
        var app = builder.Build();

        app.UseExceptionHandler();
        
        // if (app.Environment.IsDevelopment())
        // {
        app.UseOpenApi();
        // }

        app.RunMigrations<MigrationMarker>();
        
        app.UseHttpsRedirection();

        app.MapControllers();

        await app.RunAsync();
    }
}