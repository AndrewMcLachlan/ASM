using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;

namespace Asm.AspNetCore.OpenTelemetry;

internal class HttpContextTraceProcessor(IHttpContextAccessor httpContextAccessor) : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        if (httpContextAccessor.HttpContext is null) return;

        string? name = httpContextAccessor.HttpContext.GetUserName();

        data.SetCustomProperty("User", name);
    }
}
