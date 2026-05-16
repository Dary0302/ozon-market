using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OzonMarket.Api.Extensions;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApi(
        this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Введите JWT токен в формате: Bearer {your token}"
                    });
                swaggerGenOptions.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });

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
                
        var projectPrefix = "HunterDiary"; 

        var xmlFiles = Directory.EnumerateFiles(baseDirectory, "*.xml")
            .Where(file => Path.GetFileName(file).StartsWith(projectPrefix));

        foreach (var xmlPath in xmlFiles)
            options.IncludeXmlComments(xmlPath);
    }
}