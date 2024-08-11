using System.Reflection;
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
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    private static readonly System.Diagnostics.FileVersionInfo FileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.Location);

    /// <summary>
    /// Maps a metadata endpoint to /metaz.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> so that calls can be chained.</returns>
    public static IEndpointConventionBuilder MapMeta(this IEndpointRouteBuilder builder) => builder.MapMeta("metaz");

    /// <summary>
    /// Maps a metadata endpoint to the given path..
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> instance that this method extends.</param>
    /// <param name="pattern">The path for the endpoint.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> so that calls can be chained.</returns>
    public static IEndpointConventionBuilder MapMeta(this IEndpointRouteBuilder builder, string pattern) =>
            builder.MapGet(pattern, () =>
            {
                if (FileVersionInfo.FileVersion == null) return Results.NotFound();

                return Results.Ok(new MetaModel(new Version(FileVersionInfo.FileVersion!)));
            })
            .WithName("get-meta")
            .WithDisplayName("Meta")
            .WithSummary("Gets metadata for the service");

}
