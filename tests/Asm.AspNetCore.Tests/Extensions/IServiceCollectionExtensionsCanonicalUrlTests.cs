using Asm.AspNetCore.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="AsmAspNetCoreServiceCollectionExtensions.AddCanonicalUrls"/>.
/// </summary>
public class IServiceCollectionExtensionsCanonicalUrlTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Null guard
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddCanonicalUrls_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        Assert.Throws<ArgumentNullException>(() => services.AddCanonicalUrls());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Chaining
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddCanonicalUrls_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();
        var returned = services.AddCanonicalUrls();

        Assert.Same(services, returned);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // No configure → defaults resolvable via IOptions
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddCanonicalUrls_NoConfigure_RegistersDefaultOptions()
    {
        var services = new ServiceCollection();
        services.AddCanonicalUrls();

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CanonicalUrlOptions>>().Value;

        Assert.True(options.ForceLowercase);
        Assert.True(options.RemoveTrailingSlash);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Configure callback flows into resolved options
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddCanonicalUrls_WithConfigure_AppliesCustomOptions()
    {
        var services = new ServiceCollection();
        services.AddCanonicalUrls(opts =>
        {
            opts.ForceLowercase = false;
            opts.ExemptPathPrefixes = ["/umbraco"];
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CanonicalUrlOptions>>().Value;

        Assert.False(options.ForceLowercase);
        Assert.Single(options.ExemptPathPrefixes);
        Assert.Equal("/umbraco", options.ExemptPathPrefixes[0]);
    }
}
