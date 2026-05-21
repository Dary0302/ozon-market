using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProductService.Api.Extensions;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApi(
        this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.DocumentFilter<ExplicitSchemaDocumentFilter>();
                ConfigureDisplayComments(swaggerGenOptions);
            });
    }

    public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }

    private static void ConfigureDisplayComments(SwaggerGenOptions options)
    {
        var baseDirectory = AppContext.BaseDirectory;
                
        const string projectPrefix = "ProductService"; 

        var xmlFiles = Directory.EnumerateFiles(baseDirectory, "*.xml")
            .Where(file => Path.GetFileName(file).StartsWith(projectPrefix));

        foreach (var xmlPath in xmlFiles)
            options.IncludeXmlComments(xmlPath);
    }
}