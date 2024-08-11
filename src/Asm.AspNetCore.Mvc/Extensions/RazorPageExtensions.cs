using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.AspNetCore.Mvc.Razor;

/// <summary>
/// Extension methods for <see cref="RazorPage"/>.
/// </summary>
public static class RazorPageExtensions
{
    /// <summary>
    /// Renders a section if it is defined, otherwise renders a default view.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="sectionName">The name of the section.</param>
    /// <param name="html">An HTML helper instance.</param>
    /// <param name="defaultView">The default view if the section is not defined.</param>
    /// <returns>The HTML content.</returns>
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
