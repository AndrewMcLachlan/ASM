using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Asm.AspNetCore.Mvc.TagHelpers;

/// <summary>
/// A tag helper that adds the integrity attribute to a script tag.
/// </summary>
public class ScriptTagHelper : IntegrityTagHelper
{
    private string _urlAttributeName = "src";
    private readonly bool _emitMinifiedUrls = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptTagHelper"/> class.
    /// </summary>
    /// <param name="actionContextAccessor">An action context accessor.</param>
    /// <param name="urlHelperFactory">A URL helper factory.</param>
    /// <param name="hostingEnvironment">The hosting environment.</param>
    /// <param name="memoryCache">A memory cache.</param>
    /// <param name="configuration">App configuration.</param>
    /// <exception cref="InvalidOperationException">If there is no action context.</exception>
    /// <exception cref="InvalidOperationException">If the configuration value EmitMinifiedUrls is defined but is not a <see langword="bool"/>.</exception>
    public ScriptTagHelper(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, IConfiguration configuration) : base(actionContextAccessor, urlHelperFactory, hostingEnvironment, memoryCache)
    {
        if (!Boolean.TryParse(configuration["EmitMinifiedUrls"] ?? "false", out _emitMinifiedUrls)) throw new InvalidOperationException("EmitMinifiedUrls is not a boolean value");
    }

    /// <inheritdoc/>
    protected override string UrlSourceAttributeName => _urlAttributeName;

    /// <inheritdoc/>
    protected override string UrlOutputAttributeName => "src";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context.AllAttributes["integrity"] != null) return;

        _urlAttributeName = _emitMinifiedUrls && context.AllAttributes["src-min"] != null ? "src-min" : "src";

        base.Process(context, output);

        output.Attributes.RemoveAll("src-min");
    }
}
