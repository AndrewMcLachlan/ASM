using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Asm.AspNetCore.HealthChecks;

internal static class ResponseWriter
{
    public static async Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            version = AssemblyVersion.FileVersion?.FileVersion,
            info = AssemblyVersion.InformationalVersion,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                data = entry.Value.Data.Count > 0 ? entry.Value.Data : null,
            }),
            totalDuration = report.TotalDuration
        };

        await context.Response.WriteAsJsonAsync(response);

    }
}
