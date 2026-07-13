using Asm.AspNetCore.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="AsmAspNetCoreServiceCollectionExtensions.AddCanonicalUrls"/>.
/// </summary>
[Trait("Category", "Unit")]
public class IServiceCollectionExtensionsCanonicalUrlTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Null guard
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a null IServiceCollection
    /// When AddCanonicalUrls is called
    /// Then an ArgumentNullException is thrown
    /// </summary>
    [Fact]
    public void AddCanonicalUrlsNullServicesThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        Assert.Throws<ArgumentNullException>(() => services.AddCanonicalUrls());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Chaining
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a service collection
    /// When AddCanonicalUrls is called
    /// Then the same service collection instance is returned for chaining
    /// </summary>
    [Fact]
    public void AddCanonicalUrlsReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();
        var returned = services.AddCanonicalUrls();

        Assert.Same(services, returned);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // No configure → defaults resolvable via IOptions
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given AddCanonicalUrls is called without a configure callback
    /// When the CanonicalUrlOptions are resolved via IOptions
    /// Then ForceLowercase and RemoveTrailingSlash default to true
    /// </summary>
    [Fact]
    public void AddCanonicalUrlsNoConfigureRegistersDefaultOptions()
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

    /// <summary>
    /// Given AddCanonicalUrls is called with a configure callback
    /// When the CanonicalUrlOptions are resolved via IOptions
    /// Then the customised option values flow through
    /// </summary>
    [Fact]
    public void AddCanonicalUrlsWithConfigureAppliesCustomOptions()
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
