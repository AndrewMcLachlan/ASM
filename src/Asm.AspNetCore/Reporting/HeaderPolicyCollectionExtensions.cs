using Microsoft.AspNetCore.Builder;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;

namespace Asm.AspNetCore.Reporting;

/// <summary>
/// Extensions for <see cref="HeaderPolicyCollection"/> that register Asm security-reporting header policies.
/// </summary>
public static class HeaderPolicyCollectionReportingExtensions
{
    /// <summary>
    /// Registers the <c>Reporting-Endpoints</c> and <c>Report-To</c> response-header
    /// policies, sourced from the supplied <see cref="SecurityReportingOptions"/>.
    /// </summary>
    /// <remarks>
    /// Headers are emitted only on text/html responses (via NetEscapades's
    /// <c>HtmlOnlyHeaderPolicyBase</c>). The two header values are recomputed per request
    /// from the request scheme and host.
    /// </remarks>
    /// <param name="policies">The header-policy collection.</param>
    /// <param name="options">The reporting options.</param>
    /// <returns>The header-policy collection.</returns>
    public static HeaderPolicyCollection AddSecurityReportingHeaders(this HeaderPolicyCollection policies, SecurityReportingOptions options)
    {
        ArgumentNullException.ThrowIfNull(policies);
        ArgumentNullException.ThrowIfNull(options);

        var reportingEndpoints = new ReportingEndpointsHeaderPolicy(options);
        var reportTo = new ReportToHeaderPolicy(options);
        policies[reportingEndpoints.Header] = reportingEndpoints;
        policies[reportTo.Header] = reportTo;
        return policies;
    }
}
