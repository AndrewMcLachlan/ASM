using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace Asm.AspNetCore.Mvc.TagHelpers;

/// <summary>
/// A tag helper that adds the integrity attribute to a link tag.
/// </summary>
public sealed class LinkTagHelper(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache)
    : IntegrityTagHelper(actionContextAccessor, urlHelperFactory, hostingEnvironment, memoryCache)
{
    /// <summary>
    /// Gets the URL source attribute name.
    /// </summary>
    protected override string UrlSourceAttributeName => "href";

    /// <summary>
    /// Gets the URL output attribute name.
    /// </summary>
    protected override string UrlOutputAttributeName => UrlSourceAttributeName;

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context.AllAttributes["integrity"] != null || context.AllAttributes["rel"] == null || ((HtmlString)context.AllAttributes["rel"].Value).Value != "stylesheet") return;

        base.Process(context, output);
    }
}
