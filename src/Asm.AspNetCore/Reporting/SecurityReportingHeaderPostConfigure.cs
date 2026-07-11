using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Asm.AspNetCore.Reporting;

/// <summary>
/// Post-configures the standard <see cref="HeaderPolicyCollection"/> with the
/// <c>Reporting-Endpoints</c> and <c>Report-To</c> header policies.
/// </summary>
/// <remarks>
/// Registered by <c>AddSecurityReporting</c>.
/// Because it runs as an <see cref="IPostConfigureOptions{TOptions}"/> — after every
/// <c>Configure</c> callback that populates the collection — the reporting headers compose
/// correctly regardless of whether <c>AddSecurityReporting</c> or
/// <c>AddStandardSecurityHeaders</c> was registered first. This replaces the previous
/// call-order-dependent auto-coupling.
/// </remarks>
internal sealed class SecurityReportingHeaderPostConfigure(SecurityReportingOptions options) : IPostConfigureOptions<HeaderPolicyCollection>
{
    private readonly SecurityReportingOptions _options = options;

    /// <inheritdoc />
    public void PostConfigure(string? name, HeaderPolicyCollection options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.AddSecurityReportingHeaders(_options);
    }
}
