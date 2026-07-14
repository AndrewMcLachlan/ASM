using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Asm.AspNetCore.Api;

/// <summary>
/// An <see cref="IOpenApiDocumentTransformer"/> that sets the document's title and derives its version from
/// the file version of an assembly (the entry assembly by default).
/// </summary>
public sealed class DocumentInfoTransformer : IOpenApiDocumentTransformer
{
    private readonly string _title;
    private readonly Assembly _assembly;

    /// <summary>
    /// Initialises the transformer.
    /// </summary>
    /// <param name="title">The document title.</param>
    /// <param name="assembly">The assembly whose file version supplies the document version. Defaults to the entry assembly.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="title"/> is <c>null</c> or empty.</exception>
    public DocumentInfoTransformer(string title, Assembly? assembly = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(title);
        _title = title;
        _assembly = assembly ?? Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    }

    /// <inheritdoc />
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info ??= new OpenApiInfo();
        document.Info.Title = _title;

        var version = GetFileVersion(_assembly);
        if (!String.IsNullOrEmpty(version))
        {
            document.Info.Version = version;
        }

        return Task.CompletedTask;
    }

    private static string? GetFileVersion(Assembly assembly) =>
        String.IsNullOrEmpty(assembly.Location) ? null : FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
}
