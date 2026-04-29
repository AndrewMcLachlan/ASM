using Microsoft.AspNetCore.Http;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;

namespace Asm.AspNetCore.Reporting;

/// <summary>
/// Emits the <c>Reporting-Endpoints</c> response header on HTML responses,
/// using values from the supplied <see cref="SecurityReportingOptions"/>.
/// </summary>
internal sealed class ReportingEndpointsHeaderPolicy(SecurityReportingOptions options) : HtmlOnlyHeaderPolicyBase
{
    /// <inheritdoc />
    public override string Header => "Reporting-Endpoints";

    /// <inheritdoc />
    protected override string GetValue(HttpContext context) => SecurityReportingHeaderBuilder.BuildReportingEndpoints(context, options);
}
