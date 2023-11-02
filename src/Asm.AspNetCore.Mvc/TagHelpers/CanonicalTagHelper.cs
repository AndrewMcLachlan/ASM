using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asm.AspNetCore.Mvc.TagHelpers;

[HtmlTargetElement("canonical", Attributes = "path")]
public class CanonicalTagHelper : TagHelper
{
    [HtmlAttributeName("path")]
    public string? Path
    {
        get;
        set;
    }

    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        string href = $"{ViewContext!.HttpContext.Request.Scheme}://{ViewContext.HttpContext.Request.Host}/{Path}".Replace("//", "/").TrimEnd('/');

        output.TagName = "link";

        output.Attributes.Add("rel", "canonical");
        output.Attributes.Add("href", href);
    }
}
