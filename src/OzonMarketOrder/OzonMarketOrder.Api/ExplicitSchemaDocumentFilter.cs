using Microsoft.OpenApi;
using OzonMarketOrder.Application;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OzonMarketOrder.Api;

public class ExplicitSchemaDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var additionalTypes = typeof(ApplicationConfiguration).Assembly.GetTypes()
            .Where(type => type.Name.EndsWith("Dto"));

        foreach (var type in additionalTypes)
            context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
    }
}