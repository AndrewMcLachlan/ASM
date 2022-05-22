using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.AspNetCore.Mvc.Razor;

public static class RazorPageExtensions
{
    public static IHtmlContent? RenderSection(this RazorPage page, string sectionName, IHtmlHelper html, string defaultView)
    {
        if (page.IsSectionDefined(sectionName))
        {
            return page.RenderSection(sectionName);
        }
        else
        {
            return html.PartialAsync(defaultView).Result;
        }
    }
}
