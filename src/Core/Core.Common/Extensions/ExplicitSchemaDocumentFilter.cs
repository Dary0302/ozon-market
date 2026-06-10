using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Common.Extensions;

public class ExplicitSchemaDocumentFilter(Assembly assemblies) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var additionalTypes = assemblies
            .GetTypes()
            .Where(type => type.Name.EndsWith("Dto"));

        foreach (var type in additionalTypes)
        {
            context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
        }
    }
}