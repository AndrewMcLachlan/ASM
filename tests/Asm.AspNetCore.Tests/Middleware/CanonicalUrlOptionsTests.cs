using Asm.AspNetCore.Middleware;

namespace Asm.AspNetCore.Tests.Middleware;

public class CanonicalUrlOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new CanonicalUrlOptions();

        Assert.True(options.ForceLowercase);
        Assert.True(options.RemoveTrailingSlash);
        Assert.Empty(options.ExemptPathPrefixes);
        Assert.Single(options.LowercaseExcludedExtensions);
        Assert.Contains(".pdf", options.LowercaseExcludedExtensions);
    }

    [Fact]
    public void ForceLowercase_CanBeSetToFalse()
    {
        var options = new CanonicalUrlOptions { ForceLowercase = false };
        Assert.False(options.ForceLowercase);
    }

    [Fact]
    public void RemoveTrailingSlash_CanBeSetToFalse()
    {
        var options = new CanonicalUrlOptions { RemoveTrailingSlash = false };
        Assert.False(options.RemoveTrailingSlash);
    }

    [Fact]
    public void LowercaseExcludedExtensions_CanBeReplaced()
    {
        var options = new CanonicalUrlOptions
        {
            LowercaseExcludedExtensions = [".pdf", ".jpg"]
        };
        Assert.Equal(2, options.LowercaseExcludedExtensions.Count);
    }

    [Fact]
    public void ExemptPathPrefixes_CanBeConfigured()
    {
        var options = new CanonicalUrlOptions
        {
            ExemptPathPrefixes = ["/umbraco"]
        };
        Assert.Single(options.ExemptPathPrefixes);
        Assert.Equal("/umbraco", options.ExemptPathPrefixes[0]);
    }
}
