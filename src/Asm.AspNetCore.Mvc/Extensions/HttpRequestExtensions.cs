namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extension methods for <see cref="HttpRequest"/>.
/// </summary>
public static class HttpRequestExtensions
{
    private const string XForwardedHost = "X-Forwarded-Host";

    /// <summary>
    /// Gets the origin host of the request.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/> instance that this method extends.</param>
    /// <returns>The origin host of the request.</returns>
    public static string OriginHost(this HttpRequest request)
    {
        if (request.Headers.TryGetValue(XForwardedHost, out Microsoft.Extensions.Primitives.StringValues value) && !String.IsNullOrEmpty(value.FirstOrDefault()))
        {
            return value.First()!;
        }

        return request.Host.ToString();
    }

    /// <summary>
    /// Gets the URL referrer of the request.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/> instance that this method extends.</param>
    /// <returns>The URL referrer of the request.</returns>
    public static string? UrlReferrer(this HttpRequest request) => request.Headers.Referer.FirstOrDefault();

    /// <summary>
    /// Determines whether the request is an AJAX request.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/> instance that this method extends.</param>
    /// <returns><see langword="true"/> if the request is an AJAX request; otherwise, <see langword="false"/>.</returns>
    public static bool IsAjaxRequest(this HttpRequest request) => request.Headers != null && request.Headers.XRequestedWith == "XMLHttpRequest";
}
