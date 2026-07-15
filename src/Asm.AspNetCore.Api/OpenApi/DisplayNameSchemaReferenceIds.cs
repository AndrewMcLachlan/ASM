using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;

namespace Asm.AspNetCore.Api;

/// <summary>
/// Resolves OpenAPI schema reference IDs from a type's <see cref="DisplayNameAttribute"/>, falling back to
/// the framework's default algorithm. Useful for giving schemas stable, friendly names and for resolving
/// name collisions between types that would otherwise share a default reference ID.
/// </summary>
public static class DisplayNameSchemaReferenceIds
{
    /// <summary>
    /// Returns the <see cref="DisplayNameAttribute.DisplayName"/> of the object type where present, otherwise
    /// the default reference ID from <see cref="OpenApiOptions.CreateDefaultSchemaReferenceId"/>.
    /// </summary>
    /// <param name="jsonTypeInfo">The serialization metadata for the type being named.</param>
    /// <returns>The schema reference ID, or <c>null</c> to omit the schema from components.</returns>
    public static string? Resolve(JsonTypeInfo jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);

        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object)
        {
            var displayName = jsonTypeInfo.Type.GetCustomAttribute<DisplayNameAttribute>(inherit: false)?.DisplayName;
            if (!String.IsNullOrEmpty(displayName))
            {
                return displayName;
            }
        }

        return OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo);
    }
}
