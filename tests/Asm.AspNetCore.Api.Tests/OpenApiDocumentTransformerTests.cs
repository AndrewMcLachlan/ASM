using System;
using System.Threading;
using System.Threading.Tasks;
using Asm.AspNetCore.Api;
using Microsoft.OpenApi;

namespace Asm.AspNetCore.Api.Tests;

public class OpenApiDocumentTransformerTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ServerPathPrefixMovesPrefixToServerAndTrimsPaths()
    {
        var document = new OpenApiDocument { Paths = new OpenApiPaths() };
        document.Paths.Add("/api/accounts", new OpenApiPathItem());
        document.Paths.Add("/api/tags", new OpenApiPathItem());

        await new ServerPathPrefixDocumentTransformer("/api").TransformAsync(document, null!, CancellationToken.None);

        Assert.Single(document.Servers);
        Assert.Equal("/api", document.Servers[0].Url);
        Assert.True(document.Paths.ContainsKey("/accounts"));
        Assert.True(document.Paths.ContainsKey("/tags"));
        Assert.False(document.Paths.ContainsKey("/api/accounts"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ServerPathPrefixMapsBarePrefixToRoot()
    {
        var document = new OpenApiDocument { Paths = new OpenApiPaths() };
        document.Paths.Add("/api", new OpenApiPathItem());

        await new ServerPathPrefixDocumentTransformer("/api").TransformAsync(document, null!, CancellationToken.None);

        Assert.True(document.Paths.ContainsKey("/"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ServerPathPrefixRequiresANonEmptyPrefix()
    {
        Assert.Throws<ArgumentException>(() => new ServerPathPrefixDocumentTransformer(""));
        Assert.Throws<ArgumentNullException>(() => new ServerPathPrefixDocumentTransformer(null));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DocumentInfoSetsTitleAndVersionFromAssembly()
    {
        var document = new OpenApiDocument();

        // System.Private.CoreLib always carries a file version.
        await new DocumentInfoTransformer("My API", typeof(string).Assembly).TransformAsync(document, null!, CancellationToken.None);

        Assert.NotNull(document.Info);
        Assert.Equal("My API", document.Info.Title);
        Assert.False(String.IsNullOrEmpty(document.Info.Version));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DocumentInfoRequiresATitle()
    {
        Assert.Throws<ArgumentException>(() => new DocumentInfoTransformer(""));
    }
}
