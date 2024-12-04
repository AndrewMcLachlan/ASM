using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Asm.AspNetCore.OpenTelemetry;

internal class HttpContextLogProcessor(IHttpContextAccessor httpContextAccessor) : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        if (httpContextAccessor.HttpContext is null) return;

        var attributes = data.Attributes?.ToList() ?? [];

        attributes.Add(new KeyValuePair<string, object?>("User", httpContextAccessor.HttpContext.GetUserName()));

        data.Attributes = attributes;
    }
}
