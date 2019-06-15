using System.Linq;
using Asm.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asm.Web.Mvc.TagHelpers
{
    public class BodyTagHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string area = ViewContext.RouteData.Values["area"] as string;
            string controller = ViewContext.RouteData.Values["controller"] as string;
            string action = ViewContext.RouteData.Values["action"] as string;

            string className = $"{area} {controller} {action}".ToLowerInvariant();

            output.TagName = "body";

            if (output.Attributes.Any(a => a.Name == "class"))
            {
                className = className.Append(output.Attributes["class"].Value as string, " ");
                output.Attributes.Remove(output.Attributes["class"]);
            }

            output.Attributes.Add(new TagHelperAttribute("class", className));
        }
    }
}
