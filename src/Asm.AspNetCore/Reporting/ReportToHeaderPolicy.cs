using Microsoft.AspNetCore.Http;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;

namespace Asm.AspNetCore.Reporting;

/// <summary>
/// Emits the <c>Report-To</c> response header on HTML responses,
/// using values from the supplied <see cref="SecurityReportingOptions"/>.
/// </summary>
internal sealed class ReportToHeaderPolicy(SecurityReportingOptions options) : HtmlOnlyHeaderPolicyBase
{
    /// <inheritdoc />
    public override string Header => "Report-To";

    /// <inheritdoc />
    protected override string GetValue(HttpContext context) => SecurityReportingHeaderBuilder.BuildReportTo(context, options);
}
