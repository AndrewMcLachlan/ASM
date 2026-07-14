using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Asm.AspNetCore.Api;

/// <summary>
/// An <see cref="IOpenApiSchemaTransformer"/> that marks every non-nullable reference-type property as
/// <c>required</c> in the generated schema. .NET's OpenAPI generator does not infer <c>required</c> from
/// nullable-reference-type annotations, so without this a non-nullable property is reported as optional.
/// </summary>
public sealed class RequiredForNonNullableSchemaTransformer : IOpenApiSchemaTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        MarkNonNullableAsRequired(schema, context.JsonTypeInfo);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds every non-nullable reference-type property of <paramref name="jsonTypeInfo"/> to the schema's
    /// <see cref="OpenApiSchema.Required"/> set. No-op for non-object types.
    /// </summary>
    /// <param name="schema">The schema to mutate.</param>
    /// <param name="jsonTypeInfo">The serialization metadata describing the type's properties.</param>
    public static void MarkNonNullableAsRequired(OpenApiSchema schema, JsonTypeInfo jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);

        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        // NullabilityInfoContext is not thread-safe, so use a fresh instance per invocation.
        var nullabilityContext = new NullabilityInfoContext();

        foreach (var property in jsonTypeInfo.Properties)
        {
            if (property.AttributeProvider is not PropertyInfo propertyInfo)
            {
                continue;
            }

            if (nullabilityContext.Create(propertyInfo).WriteState != NullabilityState.Nullable)
            {
                schema.Required ??= new HashSet<string>();
                schema.Required.Add(property.Name);
            }
        }
    }
}
