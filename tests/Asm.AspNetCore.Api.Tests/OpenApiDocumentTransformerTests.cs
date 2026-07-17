using System;
using System.Reflection;
using System.Reflection.Emit;
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

        Assert.NotNull(document.Servers);
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
    public async Task ServerPathPrefixLeavesNonMatchingPathsUntouched()
    {
        var document = new OpenApiDocument { Paths = new OpenApiPaths() };
        document.Paths.Add("/health", new OpenApiPathItem());

        await new ServerPathPrefixDocumentTransformer("/api").TransformAsync(document, null!, CancellationToken.None);

        Assert.True(document.Paths.ContainsKey("/health"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ServerPathPrefixWithoutPathsStillSetsServer()
    {
        var document = new OpenApiDocument { Paths = null! };

        await new ServerPathPrefixDocumentTransformer("/api").TransformAsync(document, null!, CancellationToken.None);

        Assert.NotNull(document.Servers);
        Assert.Equal("/api", document.Servers[0].Url);
        Assert.Null(document.Paths);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ServerPathPrefixRequiresANonEmptyPrefix()
    {
        Assert.Throws<ArgumentException>(() => new ServerPathPrefixDocumentTransformer(""));
        Assert.Throws<ArgumentNullException>(() => new ServerPathPrefixDocumentTransformer(null!));
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
    public async Task DocumentInfoPreservesExistingInfoInstance()
    {
        var info = new OpenApiInfo { Description = "kept" };
        var document = new OpenApiDocument { Info = info };

        await new DocumentInfoTransformer("My API", typeof(string).Assembly).TransformAsync(document, null!, CancellationToken.None);

        Assert.Same(info, document.Info);
        Assert.Equal("My API", document.Info.Title);
        Assert.Equal("kept", document.Info.Description);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DocumentInfoDefaultsToEntryAssembly()
    {
        var document = new OpenApiDocument();

        // No assembly supplied — the transformer falls back to the entry/executing assembly.
        await new DocumentInfoTransformer("My API").TransformAsync(document, null!, CancellationToken.None);

        Assert.NotNull(document.Info);
        Assert.Equal("My API", document.Info.Title);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DocumentInfoOmitsVersionWhenAssemblyHasNoFileVersion()
    {
        // A dynamic assembly has an empty Location, so no file version can be read.
        var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Dynamic"), AssemblyBuilderAccess.Run);
        var document = new OpenApiDocument();

        await new DocumentInfoTransformer("My API", dynamicAssembly).TransformAsync(document, null!, CancellationToken.None);

        Assert.Equal("My API", document.Info.Title);
        Assert.True(String.IsNullOrEmpty(document.Info.Version));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DocumentInfoRequiresATitle()
    {
        Assert.Throws<ArgumentException>(() => new DocumentInfoTransformer(""));
    }
}
