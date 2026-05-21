using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Common;
using ProductService.Api.Extensions;
using ProductService.Application;

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
        
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            
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