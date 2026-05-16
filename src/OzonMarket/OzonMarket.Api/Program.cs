using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Common;
using OzonMarket.Api.Extensions;
using OzonMarket.Application;
using OzonMarket.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DbContext = OzonMarket.Infrastructure.DbContext;

namespace OzonMarket.Api;

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
            .AddInfrastructureServices(builder.Configuration)
            .AddOpenApi();

        builder.Services.AddExceptionHandler<ExceptionHandler>();
        builder.Services.AddProblemDetails(); 
        
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DbContext>();
            await db.Database.MigrateAsync();
        }

        app.UseExceptionHandler();
        
        // if (app.Environment.IsDevelopment())
        // {
        app.UseOpenApi();
        // }

        app.UseHttpsRedirection();

        app.MapControllers();

        await app.RunAsync();
    }
}