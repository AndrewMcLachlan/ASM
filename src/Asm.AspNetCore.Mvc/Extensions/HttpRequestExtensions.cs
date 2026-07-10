namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extension methods for <see cref="HttpRequest"/>.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the origin host of the request (<see cref="HttpRequest.Host"/>).
    /// </summary>
    /// <remarks>
    /// The raw <c>X-Forwarded-Host</c> header is deliberately not trusted (any client can set it,
    /// which would allow host/cache poisoning). Behind a reverse proxy, enable the Forwarded Headers
    /// middleware (<c>UseForwardedHeaders</c>) configured with your known proxies/networks so that
    /// <see cref="HttpRequest.Host"/> reflects the validated forwarded host.
    /// </remarks>
    /// <param name="request">The <see cref="HttpRequest"/> instance that this method extends.</param>
    /// <returns>The origin host of the request.</returns>
    public static string OriginHost(this HttpRequest request) => request.Host.ToString();

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
