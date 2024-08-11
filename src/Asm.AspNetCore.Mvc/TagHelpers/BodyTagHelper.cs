using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asm.AspNetCore.Mvc.TagHelpers;

/// <summary>
/// A tag helper that adds the area, controller, and action as classes to the body tag.
/// </summary>
public class BodyTagHelper : TagHelper
{
    /// <summary>
    /// Gets or sets the view context.
    /// </summary>
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        string? area = ViewContext?.RouteData.Values["area"] as string;
        string? controller = ViewContext?.RouteData.Values["controller"] as string;
        string? action = ViewContext?.RouteData.Values["action"] as string;

        string className = $"{area} {controller} {action}".ToLowerInvariant();

        output.TagName = "body";

        if (output.Attributes.Any(a => a.Name == "class"))
        {
            className = className.Append(output.Attributes["class"].Value as string ?? String.Empty, " ");
            output.Attributes.Remove(output.Attributes["class"]);
        }

        output.Attributes.Add(new TagHelperAttribute("class", className));
    }
}
