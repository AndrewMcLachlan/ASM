using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Middleware;

/// <summary>
/// Options for <see cref="SecurityHeadersMiddleware"/>.
/// </summary>
/// <remarks>
/// The middleware ships sensible defaults for the commonly-required security
/// headers. Consumers only need to override the properties that are site-specific
/// (typically <see cref="ContentSecurityPolicy"/> and <see cref="PermissionsPolicy"/>).
/// Set any property to <see langword="null"/> to suppress that header entirely.
/// </remarks>
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
    /// <c>Content-Security-Policy</c> header value. Defaults to <c>"default-src 'self'"</c>.
    /// Set to <see langword="null"/> to suppress the header. Most apps should override this
    /// with a policy tailored to their resource origins.
    /// </summary>
    public string? ContentSecurityPolicy { get; set; } = "default-src 'self'";

    /// <summary>
    /// <c>Cross-Origin-Opener-Policy</c> header value. Defaults to
    /// <c>"same-origin-allow-popups"</c> to allow third-party OAuth popups.
    /// Set to <see langword="null"/> to suppress the header.
    /// </summary>
    public string? CrossOriginOpenerPolicy { get; set; } = "same-origin-allow-popups";

    /// <summary>
    /// <c>Cross-Origin-Embedder-Policy</c> header value. Defaults to <c>"require-corp"</c>.
    /// Set to <see langword="null"/> to suppress the header.
    /// </summary>
    public string? CrossOriginEmbedderPolicy { get; set; } = "require-corp";

    /// <summary>
    /// <c>Cross-Origin-Resource-Policy</c> header value. Defaults to <c>"same-origin"</c>.
    /// Set to <see langword="null"/> to suppress the header.
    /// </summary>
    public string? CrossOriginResourcePolicy { get; set; } = "same-origin";

    /// <summary>
    /// <c>X-Frame-Options</c> header value. Defaults to <c>"SAMEORIGIN"</c>.
    /// Set to <see langword="null"/> to suppress the header.
    /// </summary>
    public string? XFrameOptions { get; set; } = "SAMEORIGIN";

    /// <summary>
    /// <c>X-Content-Type-Options</c> header value. Defaults to <c>"nosniff"</c>.
    /// Set to <see langword="null"/> to suppress the header.
    /// </summary>
    public string? XContentTypeOptions { get; set; } = "nosniff";

    /// <summary>
    /// <c>Referrer-Policy</c> header value. Defaults to <c>"strict-origin-when-cross-origin"</c>.
    /// Set to <see langword="null"/> to suppress the header.
    /// </summary>
    public string? ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";

    /// <summary>
    /// <c>Permissions-Policy</c> header value. Defaults to <see langword="null"/>
    /// (no universally-sensible default); consumers needing the header should set one.
    /// </summary>
    public string? PermissionsPolicy { get; set; }

    /// <summary>
    /// <c>X-Permitted-Cross-Domain-Policies</c> header value. Defaults to <c>"none"</c>,
    /// instructing legacy Flash/Acrobat clients not to load cross-domain policy files.
    /// Set to <see langword="null"/> to suppress the header.
    /// </summary>
    public string? XPermittedCrossDomainPolicies { get; set; } = "none";

    /// <summary>
    /// Additional headers beyond the standard set exposed as first-class properties.
    /// Useful for non-standard or consumer-specific headers.
    /// </summary>
    public IDictionary<string, string> CustomHeaders { get; set; }
        = new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>
    /// Optional callback producing additional headers computed per-request (e.g., containing
    /// the request scheme or host). Merged after <see cref="CustomHeaders"/>.
    /// </summary>
    public Func<HttpContext, IDictionary<string, string>>? DynamicHeaders { get; set; }
}
