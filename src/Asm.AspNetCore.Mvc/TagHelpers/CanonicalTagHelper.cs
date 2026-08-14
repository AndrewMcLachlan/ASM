using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asm.AspNetCore.Mvc.TagHelpers;

/// <summary>
/// Outputs a canonical link tag.
/// </summary>

[HtmlTargetElement("canonical", Attributes = "path")]
public class CanonicalTagHelper : TagHelper
{
    /// <summary>
    /// Gets or sets the canonical URL.
    /// </summary>
    /// <remarks>
    /// Accepts either an absolute URL, which is emitted unchanged, or a site-relative path, which
    /// is resolved against the current request's scheme and host.
    /// </remarks>
    [HtmlAttributeName("path")]
    public required string Path
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the view context.
    /// </summary>
    [ViewContext]
    public required ViewContext ViewContext { get; set; }

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "link";

        output.Attributes.Add("rel", "canonical");
        output.Attributes.Add("href", ResolveHref());
    }

    // A canonical URL is absolute by definition, so callers may supply one directly. Only a
    // relative path needs the request's scheme and host grafted on.
    private string ResolveHref()
    {
        if (Uri.TryCreate(Path, UriKind.Absolute, out var absolute) &&
            (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
        {
            return Path.TrimEnd('/');
        }

        var request = ViewContext.HttpContext.Request;

        return $"{request.Scheme}://{request.Host}/{Path.TrimStart('/')}".TrimEnd('/');
    }
}
