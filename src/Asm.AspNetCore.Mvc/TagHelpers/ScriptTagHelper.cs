using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asm.AspNetCore.Mvc.TagHelpers;

/// <summary>
/// A tag helper that adds the integrity attribute to a script tag.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ScriptTagHelper"/> class.
/// </remarks>
/// <param name="urlHelperFactory">A URL helper factory.</param>
/// <param name="hostingEnvironment">The hosting environment.</param>
/// <param name="memoryCache">A memory cache.</param>
/// <param name="configuration">App configuration.</param>
/// <param name="logger">Logger for this tag helper.</param>
/// <exception cref="InvalidOperationException">If there is no action context.</exception>
/// <exception cref="InvalidOperationException">If the configuration value EmitMinifiedUrls is defined but is not a <see langword="bool"/>.</exception>
public class ScriptTagHelper(IUrlHelperFactory urlHelperFactory, IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache, IConfiguration configuration, ILogger<ScriptTagHelper> logger) : IntegrityTagHelper(urlHelperFactory, hostingEnvironment, memoryCache, logger)
{
    private string _urlAttributeName = "src";
    private readonly bool _emitMinifiedUrls = configuration.GetValue("EmitMinifiedUrls", false);

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
