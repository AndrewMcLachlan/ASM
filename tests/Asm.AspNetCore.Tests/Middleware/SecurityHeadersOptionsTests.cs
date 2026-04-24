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
        Assert.Equal("default-src 'self'", options.ContentSecurityPolicy);
        Assert.Equal("same-origin-allow-popups", options.CrossOriginOpenerPolicy);
        Assert.Equal("require-corp", options.CrossOriginEmbedderPolicy);
        Assert.Equal("same-origin", options.CrossOriginResourcePolicy);
        Assert.Equal("SAMEORIGIN", options.XFrameOptions);
        Assert.Equal("nosniff", options.XContentTypeOptions);
        Assert.Equal("strict-origin-when-cross-origin", options.ReferrerPolicy);
        Assert.Null(options.PermissionsPolicy);
        Assert.Equal("none", options.XPermittedCrossDomainPolicies);
        Assert.Empty(options.CustomHeaders);
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

    [Theory]
    [InlineData(nameof(SecurityHeadersOptions.ContentSecurityPolicy), "script-src 'self' cdn.example.com")]
    [InlineData(nameof(SecurityHeadersOptions.CrossOriginOpenerPolicy), "same-origin")]
    [InlineData(nameof(SecurityHeadersOptions.CrossOriginEmbedderPolicy), "unsafe-none")]
    [InlineData(nameof(SecurityHeadersOptions.CrossOriginResourcePolicy), "cross-origin")]
    [InlineData(nameof(SecurityHeadersOptions.XFrameOptions), "DENY")]
    [InlineData(nameof(SecurityHeadersOptions.XContentTypeOptions), "nosniff")]
    [InlineData(nameof(SecurityHeadersOptions.ReferrerPolicy), "no-referrer")]
    [InlineData(nameof(SecurityHeadersOptions.PermissionsPolicy), "geolocation=()")]
    [InlineData(nameof(SecurityHeadersOptions.XPermittedCrossDomainPolicies), "master-only")]
    public void HeaderProperties_CanBeOverridden(string propertyName, string value)
    {
        var options = new SecurityHeadersOptions();
        var prop = typeof(SecurityHeadersOptions).GetProperty(propertyName)!;
        prop.SetValue(options, value);
        Assert.Equal(value, prop.GetValue(options));
    }

    [Theory]
    [InlineData(nameof(SecurityHeadersOptions.ContentSecurityPolicy))]
    [InlineData(nameof(SecurityHeadersOptions.CrossOriginOpenerPolicy))]
    [InlineData(nameof(SecurityHeadersOptions.CrossOriginEmbedderPolicy))]
    [InlineData(nameof(SecurityHeadersOptions.CrossOriginResourcePolicy))]
    [InlineData(nameof(SecurityHeadersOptions.XFrameOptions))]
    [InlineData(nameof(SecurityHeadersOptions.XContentTypeOptions))]
    [InlineData(nameof(SecurityHeadersOptions.ReferrerPolicy))]
    [InlineData(nameof(SecurityHeadersOptions.PermissionsPolicy))]
    [InlineData(nameof(SecurityHeadersOptions.XPermittedCrossDomainPolicies))]
    public void HeaderProperties_CanBeSetToNull(string propertyName)
    {
        var options = new SecurityHeadersOptions();
        var prop = typeof(SecurityHeadersOptions).GetProperty(propertyName)!;
        prop.SetValue(options, null);
        Assert.Null(prop.GetValue(options));
    }

    [Fact]
    public void CustomHeaders_CanBePopulated()
    {
        var options = new SecurityHeadersOptions();
        options.CustomHeaders["X-Custom"] = "value";
        Assert.Equal("value", options.CustomHeaders["X-Custom"]);
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
