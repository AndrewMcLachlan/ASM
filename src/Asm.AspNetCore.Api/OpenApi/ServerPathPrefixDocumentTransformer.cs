using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Asm.AspNetCore.Api;

/// <summary>
/// An <see cref="IOpenApiDocumentTransformer"/> that moves a common path prefix (e.g. <c>/api</c>) into a
/// relative server URL and strips it from every operation path, so the document reads as <c>/accounts</c>
/// rather than <c>/api/accounts</c>. The relative server is resolved by clients (Swagger UI, generated
/// SDKs) against the current origin, so requests still reach <c>/api/accounts</c>.
/// </summary>
public sealed class ServerPathPrefixDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly string _pathPrefix;

    /// <summary>
    /// Initialises the transformer.
    /// </summary>
    /// <param name="pathPrefix">The path prefix to relocate, e.g. <c>/api</c>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="pathPrefix"/> is <c>null</c> or empty.</exception>
    public ServerPathPrefixDocumentTransformer(string pathPrefix)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathPrefix);
        _pathPrefix = pathPrefix;
    }

    /// <inheritdoc />
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Servers = [new OpenApiServer { Url = _pathPrefix }];

        if (document.Paths is null)
        {
            return Task.CompletedTask;
        }

        var rewritten = new OpenApiPaths();
        foreach (var (path, item) in document.Paths)
        {
            var trimmed = path.StartsWith(_pathPrefix, StringComparison.OrdinalIgnoreCase) ? path[_pathPrefix.Length..] : path;
            if (trimmed.Length == 0)
            {
                trimmed = "/";
            }

            rewritten.Add(trimmed, item);
        }

        document.Paths = rewritten;

        return Task.CompletedTask;
    }
}
