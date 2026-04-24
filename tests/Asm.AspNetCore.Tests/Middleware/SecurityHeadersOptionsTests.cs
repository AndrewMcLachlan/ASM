using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.Middleware;

public class SecurityHeadersOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new SecurityHeadersOptions();

        Assert.True(options.RemoveServerHeaders);
        Assert.Empty(options.ExemptPathPrefixes);
        Assert.Empty(options.Headers);
        Assert.Null(options.DynamicHeaders);
    }

    [Fact]
    public void RemoveServerHeaders_CanBeSetToFalse()
    {
        var options = new SecurityHeadersOptions { RemoveServerHeaders = false };
        Assert.False(options.RemoveServerHeaders);
    }

    [Fact]
    public void ExemptPathPrefixes_CanBeConfigured()
    {
        var options = new SecurityHeadersOptions
        {
            ExemptPathPrefixes = ["/api", "/health"]
        };
        Assert.Equal(2, options.ExemptPathPrefixes.Count);
        Assert.Contains("/api", options.ExemptPathPrefixes);
    }

    [Fact]
    public void Headers_CanBePopulated()
    {
        var options = new SecurityHeadersOptions();
        options.Headers["X-Custom"] = "value";
        Assert.Equal("value", options.Headers["X-Custom"]);
    }

    [Fact]
    public void DynamicHeaders_CanBeAssigned()
    {
        Func<HttpContext, IDictionary<string, string>> cb =
            _ => new Dictionary<string, string> { ["X-Dyn"] = "yes" };

        var options = new SecurityHeadersOptions { DynamicHeaders = cb };
        Assert.NotNull(options.DynamicHeaders);
    }
}
