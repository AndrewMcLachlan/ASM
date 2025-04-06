using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Asm.AspNetCore.Mvc.TagHelpers;

/// <summary>
/// A tag helper that adds the integrity attribute to a tag.
/// </summary>
public abstract class IntegrityTagHelper : TagHelper
{
    private readonly ILogger<IntegrityTagHelper> _logger;

    /// <summary>
    /// Gets or sets the view context.
    /// </summary>
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    /// <summary>
    /// Gets the URL helper.
    /// </summary>
    protected IUrlHelper UrlHelper { get; }

    /// <summary>
    /// Gets the hosting environment.
    /// </summary>
    protected IWebHostEnvironment HostingEnvironment { get; }

    /// <summary>
    /// Gets the memory cache.
    /// </summary>
    protected IMemoryCache MemoryCache { get; }

    /// <summary>
    /// Gets the URL source attribute name.
    /// </summary>
    protected abstract string UrlSourceAttributeName { get; }

    /// <summary>
    /// Gets the URL output attribute name.
    /// </summary>
    protected abstract string UrlOutputAttributeName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrityTagHelper"/> class.
    /// </summary>
    /// <param name="actionContextAccessor">An action context accessor.</param>
    /// <param name="urlHelperFactory">A URL helper factory.</param>
    /// <param name="hostingEnvironment">The hosting environment.</param>
    /// <param name="memoryCache">A memory cache.</param>
    /// <param name="logger">Logger for this tag helper.</param>
    /// <exception cref="InvalidOperationException">If there is no action context.</exception>
    public IntegrityTagHelper(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, ILogger<IntegrityTagHelper> logger)
    {
        if (actionContextAccessor.ActionContext == null) throw new InvalidOperationException("No ActionContext provided.");

        UrlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        HostingEnvironment = hostingEnvironment;
        MemoryCache = memoryCache;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var urlHtmlString = context.AllAttributes[UrlSourceAttributeName]?.Value as HtmlString;
        var url = urlHtmlString?.Value ?? context.AllAttributes[UrlSourceAttributeName]?.Value as string;

        if (String.IsNullOrEmpty(url)) return;

        if (!HostingEnvironment.IsDevelopment() && MemoryCache.TryGetValue(url, out (string Url, string Hash) data))
        {
            output.Attributes.RemoveAll(UrlSourceAttributeName);
            output.Attributes.RemoveAll(UrlOutputAttributeName);
            output.Attributes.Add(UrlOutputAttributeName, data.Url);
            output.Attributes.Add("integrity", data.Hash);
            return;
        }

        string cleanPath = url.Replace('~', '.');
        cleanPath = cleanPath[..(cleanPath.IndexOf('?') > 0 ? cleanPath.IndexOf('?') : cleanPath.Length)];
        cleanPath = cleanPath.Replace('/', Path.DirectorySeparatorChar);

        string path = Path.Combine(HostingEnvironment.WebRootPath, cleanPath);

        if (!File.Exists(path))
        {
            _logger.LogWarning("Integrity Tag Helper - File not found: {Path}", path);
            return;
        }

        var hashAlgo = System.Security.Cryptography.SHA512.Create() ?? throw new InvalidOperationException("Unable to create SHA-512 hash algorithm");

        using FileStream file = File.OpenRead(path);
        byte[] hash = hashAlgo.ComputeHash(file);

        string hashBase64 = "sha512-" + Convert.ToBase64String(hash);
        string calculatedUrl = UrlHelper.Content(url.Replace("$v", Math.Abs(hashBase64.GetHashCode()).ToString()));

        MemoryCache.Set(url, (calculatedUrl, hashBase64));

        output.Attributes.RemoveAll(UrlSourceAttributeName);
        output.Attributes.RemoveAll(UrlOutputAttributeName);

        output.Attributes.Add(UrlOutputAttributeName, calculatedUrl);
        output.Attributes.Add("integrity", hashBase64);
    }
}
