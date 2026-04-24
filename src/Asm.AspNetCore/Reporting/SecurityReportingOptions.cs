namespace Asm.AspNetCore.Reporting;

/// <summary>
/// Options governing the security-reporting endpoints and the associated
/// <c>Reporting-Endpoints</c> / <c>Report-To</c> response headers.
/// </summary>
public record SecurityReportingOptions
{
    /// <summary>
    /// Route prefix for the reporting endpoints. Defaults to <c>"reporting"</c>.
    /// </summary>
    public string RoutePrefix { get; set; } = "reporting";

    /// <summary>
    /// Route segment for the integrity-violation endpoint. Appended to <see cref="RoutePrefix"/>.
    /// </summary>
    public string IntegrityRoute { get; set; } = "integrity";

    /// <summary>
    /// Route segment for the Content Security Policy report endpoint. Appended to <see cref="RoutePrefix"/>.
    /// </summary>
    public string CspRoute { get; set; } = "csp";

    /// <summary>
    /// Group name used for the integrity reporting endpoint in the
    /// <c>Reporting-Endpoints</c> header and the <c>Report-To</c> group entry.
    /// </summary>
    public string IntegrityGroupName { get; set; } = "integrity-endpoint";

    /// <summary>
    /// Group name used for the CSP reporting endpoint in the
    /// <c>Reporting-Endpoints</c> header and the <c>Report-To</c> group entry.
    /// </summary>
    public string CspGroupName { get; set; } = "csp-endpoint";

    /// <summary>
    /// <c>max_age</c> value (in seconds) emitted in the <c>Report-To</c> groups. Defaults to 24 hours.
    /// </summary>
    public int MaxAgeSeconds { get; set; } = 86400;

    /// <summary>
    /// Maximum accepted request body size for a single report, in bytes.
    /// Requests exceeding this are rejected with HTTP 413 and no body is logged.
    /// Defaults to 65536 (64 KiB), which is generous for standards-compliant CSP reports.
    /// </summary>
    public int MaxBodyBytes { get; set; } = 65536;
}
