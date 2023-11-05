using System.Reflection;
using Asm.AspNetCore.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Routing;
public static class IEndpointRouteBuilderExtensions
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    private static readonly System.Diagnostics.FileVersionInfo FileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.Location);

    public static IEndpointConventionBuilder MapMeta(this IEndpointRouteBuilder builder) => builder.MapMeta("metaz");

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
