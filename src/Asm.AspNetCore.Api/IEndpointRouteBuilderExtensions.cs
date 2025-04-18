using Asm.AspNetCore.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Routing;

/// <summary>
/// Extension methods for <see cref="IEndpointRouteBuilder"/>.
/// </summary>
public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a metadata endpoint to /metaz.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> so that calls can be chained.</returns>
    [Obsolete("Use HealthChecks for metadata")]
    public static IEndpointConventionBuilder MapMeta(this IEndpointRouteBuilder builder) => builder.MapMeta("metaz");

    /// <summary>
    /// Maps a metadata endpoint to the given path..
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> instance that this method extends.</param>
    /// <param name="pattern">The path for the endpoint.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> so that calls can be chained.</returns>
    [Obsolete("Use HealthChecks for metadata")]
    public static IEndpointConventionBuilder MapMeta(this IEndpointRouteBuilder builder, string pattern) =>
            builder.MapGet(pattern, () =>
            {
                if (AssemblyVersion.FileVersion == null) return Results.NotFound();

                return Results.Ok(new MetaModel(new Version(AssemblyVersion.FileVersion.FileVersion!)));
            })
            .WithName("get-meta")
            .WithDisplayName("Meta")
            .WithSummary("Gets metadata for the service");

}
