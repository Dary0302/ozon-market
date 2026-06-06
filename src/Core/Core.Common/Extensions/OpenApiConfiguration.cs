using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Common.Extensions;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApi(
        this IServiceCollection services,
        string projectPrefix,
        Type configurationType)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.DocumentFilter<ExplicitSchemaDocumentFilter>();

                services.AddSingleton<IDocumentFilter>(_ =>
                    new ExplicitSchemaDocumentFilter(configurationType.Assembly));
                ConfigureDisplayComments(swaggerGenOptions, projectPrefix);
            });
    }

    public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }

    private static void ConfigureDisplayComments(SwaggerGenOptions options, string projectPrefix)
    {
        var baseDirectory = AppContext.BaseDirectory;

        var xmlFiles = Directory.EnumerateFiles(baseDirectory, "*.xml")
            .Where(file => Path.GetFileName(file).StartsWith(projectPrefix));

        foreach (var xmlPath in xmlFiles)
            options.IncludeXmlComments(xmlPath);
    }
}