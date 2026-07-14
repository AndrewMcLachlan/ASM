using Asm.AspNetCore.Reporting;

namespace Asm.AspNetCore.Tests.Reporting;

[Trait("Category", "Unit")]

public class SecurityReportingOptionsTests
{
    /// <summary>
    /// Given a newly constructed SecurityReportingOptions
    /// When its default values are inspected
    /// Then the default routes, group names, max-age and max-body-bytes are set
    /// </summary>
    [Fact]
    public void DefaultValuesAreCorrect()
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

    /// <summary>
    /// Given a SecurityReportingOptions with RoutePrefix changed
    /// When the property is read
    /// Then it reports the new value
    /// </summary>
    [Fact]
    public void RoutePrefixCanBeChanged()
    {
        var options = new SecurityReportingOptions { RoutePrefix = "api/reporting" };
        Assert.Equal("api/reporting", options.RoutePrefix);
    }

    /// <summary>
    /// Given a SecurityReportingOptions with MaxAgeSeconds changed
    /// When the property is read
    /// Then it reports the new value
    /// </summary>
    [Fact]
    public void MaxAgeSecondsCanBeChanged()
    {
        var options = new SecurityReportingOptions { MaxAgeSeconds = 3600 };
        Assert.Equal(3600, options.MaxAgeSeconds);
    }

    /// <summary>
    /// Given a SecurityReportingOptions with the group names changed
    /// When the properties are read
    /// Then they report the new values
    /// </summary>
    [Fact]
    public void GroupNamesCanBeChanged()
    {
        var options = new SecurityReportingOptions
        {
            IntegrityGroupName = "my-integrity",
            CspGroupName = "my-csp"
        };
        Assert.Equal("my-integrity", options.IntegrityGroupName);
        Assert.Equal("my-csp", options.CspGroupName);
    }

    /// <summary>
    /// Given a SecurityReportingOptions with the route segments changed
    /// When the properties are read
    /// Then they report the new values
    /// </summary>
    [Fact]
    public void RouteSegmentsCanBeChanged()
    {
        var options = new SecurityReportingOptions
        {
            IntegrityRoute = "sri",
            CspRoute = "content-security-policy"
        };
        Assert.Equal("sri", options.IntegrityRoute);
        Assert.Equal("content-security-policy", options.CspRoute);
    }

    /// <summary>
    /// Given a newly constructed SecurityReportingOptions
    /// When MaxBodyBytes is read
    /// Then it defaults to 65536
    /// </summary>
    [Fact]
    public void MaxBodyBytesDefaultsTo65536()
    {
        var options = new SecurityReportingOptions();
        Assert.Equal(65536, options.MaxBodyBytes);
    }

    /// <summary>
    /// Given a SecurityReportingOptions with MaxBodyBytes changed
    /// When the property is read
    /// Then it reports the new value
    /// </summary>
    [Fact]
    public void MaxBodyBytesCanBeChanged()
    {
        var options = new SecurityReportingOptions { MaxBodyBytes = 1024 };
        Assert.Equal(1024, options.MaxBodyBytes);
    }
}
