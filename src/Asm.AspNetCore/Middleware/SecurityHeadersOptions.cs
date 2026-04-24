using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Middleware;

/// <summary>
/// Options for <see cref="SecurityHeadersMiddleware"/>.
/// </summary>
public record SecurityHeadersOptions
{
    /// <summary>
    /// When true, removes server-fingerprint response headers
    /// (<c>X-Powered-By</c>, <c>X-AspNet-Version</c>, <c>X-AspNetMvc-Version</c>, <c>Server</c>).
    /// Defaults to true.
    /// </summary>
    public bool RemoveServerHeaders { get; set; } = true;

    /// <summary>
    /// Request paths (by prefix) that should bypass security-header processing entirely.
    /// Match is case-insensitive. Defaults to empty.
    /// </summary>
    public IReadOnlyList<string> ExemptPathPrefixes { get; set; } = [];

    /// <summary>
    /// Static headers added to all HTML responses (after fingerprint stripping).
    /// </summary>
    public IDictionary<string, string> Headers { get; set; }
        = new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>
    /// Optional callback producing additional headers computed per-request (e.g., containing
    /// the request scheme or host). Merged after <see cref="Headers"/>.
    /// </summary>
    public Func<HttpContext, IDictionary<string, string>>? DynamicHeaders { get; set; }
}
