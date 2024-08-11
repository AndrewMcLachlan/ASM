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
    /// Gets or sets the path to the canonical URL.
    /// </summary>
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
        string href = $"{ViewContext.HttpContext.Request.Scheme}://{ViewContext.HttpContext.Request.OriginHost()}" + ($"/{Path}".Replace("//", "/").TrimEnd('/'));

        output.TagName = "link";

        output.Attributes.Add("rel", "canonical");
        output.Attributes.Add("href", href);
    }
}
