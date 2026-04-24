using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Asm.AspNetCore.Middleware;

/// <summary>
/// Enforces a canonical URL form — lowercase path, no trailing slash — via 301 redirects.
/// </summary>
public class CanonicalUrlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CanonicalUrlOptions _options;

    /// <summary>
    /// Creates a new <see cref="CanonicalUrlMiddleware"/>.
    /// </summary>
    public CanonicalUrlMiddleware(RequestDelegate next, IOptions<CanonicalUrlOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    /// <summary>
    /// Processes the request, emitting 301 redirects to enforce the canonical URL form.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        var queryString = context.Request.QueryString.Value;

        if (IsExempt(path) || string.IsNullOrEmpty(path))
        {
            await _next(context);
            return;
        }

        if (_options.ForceLowercase && HasUpper(path) && !HasExcludedExtension(path))
        {
            context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            context.Response.Headers.Location = path.ToLowerInvariant() + queryString;
            return;
        }

        if (_options.RemoveTrailingSlash && path.Length > 1 && path.EndsWith('/'))
        {
            var trimmed = path.TrimEnd('/');
            var physicalPath = context.Request.PathBase.Add(context.Request.Path).Value;
            var isDirectory = Directory.Exists(physicalPath);
            var isFile = File.Exists(physicalPath);

            if (!isDirectory && !isFile)
            {
                context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Response.Headers.Location = trimmed + queryString;
                return;
            }
        }

        await _next(context);
    }

    private bool IsExempt(string? path)
    {
        if (string.IsNullOrEmpty(path) || _options.ExemptPathPrefixes.Count == 0)
        {
            return false;
        }

        foreach (var prefix in _options.ExemptPathPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private static bool HasUpper(string path)
    {
        foreach (var c in path)
        {
            if (char.IsUpper(c)) return true;
        }
        return false;
    }

    private bool HasExcludedExtension(string path)
    {
        foreach (var ext in _options.LowercaseExcludedExtensions)
        {
            if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
