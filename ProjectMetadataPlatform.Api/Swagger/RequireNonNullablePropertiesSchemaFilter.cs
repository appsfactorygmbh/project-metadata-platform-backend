using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ProjectMetadataPlatform.Api.Swagger;

/// <summary>
/// Makes all non-nullable properties required in the open api schema.
/// </summary>
/// <remarks>
/// This is necessary because Swagger does not automatically detect nullable properties.
/// </remarks>
public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Add to schema.Required all properties where Nullable is false.
    /// </summary>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var additionalRequiredProps = schema.Properties
            .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key) && !IsCreatedResponse(x))
            .Select(x => x.Key);
        foreach (var propKey in additionalRequiredProps)
        {
            schema.Required.Add(propKey);
        }
    }

    /// <summary>
    /// Check if the response is a created response.
    /// </summary>
    public static bool IsCreatedResponse(KeyValuePair<string, OpenApiSchema> x)
    {
        return x.Key == "201";
    }
}
