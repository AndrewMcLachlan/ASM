using Asm.AspNetCore.Reporting;

namespace Asm.AspNetCore.Tests.Reporting;

public class SecurityReportingOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new SecurityReportingOptions();

        Assert.Equal("reporting", options.RoutePrefix);
        Assert.Equal("integrity", options.IntegrityRoute);
        Assert.Equal("csp", options.CspRoute);
        Assert.Equal("integrity-endpoint", options.IntegrityGroupName);
        Assert.Equal("csp-endpoint", options.CspGroupName);
        Assert.Equal(86400, options.MaxAgeSeconds);
        Assert.Equal(65536, options.MaxBodyBytes);
    }

    [Fact]
    public void RoutePrefix_CanBeChanged()
    {
        var options = new SecurityReportingOptions { RoutePrefix = "api/reporting" };
        Assert.Equal("api/reporting", options.RoutePrefix);
    }

    [Fact]
    public void MaxAgeSeconds_CanBeChanged()
    {
        var options = new SecurityReportingOptions { MaxAgeSeconds = 3600 };
        Assert.Equal(3600, options.MaxAgeSeconds);
    }

    [Fact]
    public void GroupNames_CanBeChanged()
    {
        var options = new SecurityReportingOptions
        {
            IntegrityGroupName = "my-integrity",
            CspGroupName = "my-csp"
        };
        Assert.Equal("my-integrity", options.IntegrityGroupName);
        Assert.Equal("my-csp", options.CspGroupName);
    }

    [Fact]
    public void RouteSegments_CanBeChanged()
    {
        var options = new SecurityReportingOptions
        {
            IntegrityRoute = "sri",
            CspRoute = "content-security-policy"
        };
        Assert.Equal("sri", options.IntegrityRoute);
        Assert.Equal("content-security-policy", options.CspRoute);
    }

    [Fact]
    public void MaxBodyBytes_DefaultsTo65536()
    {
        var options = new SecurityReportingOptions();
        Assert.Equal(65536, options.MaxBodyBytes);
    }

    [Fact]
    public void MaxBodyBytes_CanBeChanged()
    {
        var options = new SecurityReportingOptions { MaxBodyBytes = 1024 };
        Assert.Equal(1024, options.MaxBodyBytes);
    }
}
