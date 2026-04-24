namespace Asm.AspNetCore.Middleware;

/// <summary>
/// Options for <see cref="CanonicalUrlMiddleware"/>.
/// </summary>
public record CanonicalUrlOptions
{
    /// <summary>
    /// When true, redirects paths containing uppercase characters to their lowercase form
    /// (using 301 Moved Permanently).
    /// </summary>
    public bool ForceLowercase { get; set; } = true;

    /// <summary>
    /// File extensions that are exempt from lowercase redirection.
    /// Matched case-insensitively; leading dot required.
    /// </summary>
    public IReadOnlyList<string> LowercaseExcludedExtensions { get; set; } = [".pdf"];

    /// <summary>
    /// When true, redirects paths ending in <c>/</c> to their trimmed form,
    /// unless the path resolves to a physical file or directory.
    /// </summary>
    public bool RemoveTrailingSlash { get; set; } = true;

    /// <summary>
    /// Request path prefixes that bypass canonicalisation entirely.
    /// Match is case-insensitive.
    /// </summary>
    public IReadOnlyList<string> ExemptPathPrefixes { get; set; } = [];
}
