using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectMetadataPlatform.Api.Swagger;

/// <summary>
/// Makes all non-nullable properties required in the open api schema.
/// </summary>
public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var additionalRequiredProps = schema.Properties
            .Where(pair => !pair.Value.Nullable && !schema.Required.Contains(pair.Key))
            .Select(pair => pair.Key);

        foreach (var propertyKey in additionalRequiredProps)
        {
            schema.Required.Add(propertyKey);
        }
    }
}
