using Asm.AspNetCore.Middleware;

namespace Asm.AspNetCore.Tests.Middleware;

[Trait("Category", "Unit")]

public class CanonicalUrlOptionsTests
{
    /// <summary>
    /// Given a newly constructed CanonicalUrlOptions
    /// When its default values are inspected
    /// Then ForceLowercase and RemoveTrailingSlash are true and .pdf is the only excluded extension
    /// </summary>
    [Fact]
    public void DefaultValuesAreCorrect()
    {
        var options = new CanonicalUrlOptions();

        Assert.True(options.ForceLowercase);
        Assert.True(options.RemoveTrailingSlash);
        Assert.Empty(options.ExemptPathPrefixes);
        Assert.Single(options.LowercaseExcludedExtensions);
        Assert.Contains(".pdf", options.LowercaseExcludedExtensions);
    }

    /// <summary>
    /// Given a CanonicalUrlOptions with ForceLowercase set to false
    /// When the property is read
    /// Then it reports false
    /// </summary>
    [Fact]
    public void ForceLowercaseCanBeSetToFalse()
    {
        var options = new CanonicalUrlOptions { ForceLowercase = false };
        Assert.False(options.ForceLowercase);
    }

    /// <summary>
    /// Given a CanonicalUrlOptions with RemoveTrailingSlash set to false
    /// When the property is read
    /// Then it reports false
    /// </summary>
    [Fact]
    public void RemoveTrailingSlashCanBeSetToFalse()
    {
        var options = new CanonicalUrlOptions { RemoveTrailingSlash = false };
        Assert.False(options.RemoveTrailingSlash);
    }

    /// <summary>
    /// Given a CanonicalUrlOptions with LowercaseExcludedExtensions replaced
    /// When the collection is read
    /// Then it contains the replacement entries
    /// </summary>
    [Fact]
    public void LowercaseExcludedExtensionsCanBeReplaced()
    {
        var options = new CanonicalUrlOptions
        {
            LowercaseExcludedExtensions = [".pdf", ".jpg"]
        };
        Assert.Equal(2, options.LowercaseExcludedExtensions.Count);
    }

    /// <summary>
    /// Given a CanonicalUrlOptions with ExemptPathPrefixes configured
    /// When the collection is read
    /// Then it contains the configured prefix
    /// </summary>
    [Fact]
    public void ExemptPathPrefixesCanBeConfigured()
    {
        var options = new CanonicalUrlOptions
        {
            ExemptPathPrefixes = ["/umbraco"]
        };
        Assert.Single(options.ExemptPathPrefixes);
        Assert.Equal("/umbraco", options.ExemptPathPrefixes[0]);
    }
}
