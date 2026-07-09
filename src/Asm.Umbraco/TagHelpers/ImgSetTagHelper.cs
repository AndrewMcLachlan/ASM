using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Asm.Umbraco.TagHelpers;

/// <summary>
/// Renders an <c>&lt;img&gt;</c> element with a <c>srcset</c> attribute built from a
/// collection of Umbraco <see cref="IPublishedContent"/> media items, using the first item
/// as the base image and subsequent items (with a <c>scaling</c> property) as additional
/// srcset entries. Caches the image dimensions in memory when they aren't supplied by Umbraco.
/// </summary>
[HtmlTargetElement("imgset", Attributes = "images")]
public class ImgSetTagHelper(IWebHostEnvironment webHostEnvironment, IMemoryCache memoryCache) : TagHelper
{
    /// <summary>
    /// The current view context.
    /// </summary>
    [ViewContext]
    public required ViewContext ViewContext { get; set; }

    /// <summary>
    /// The collection of images. The first is the base image; each subsequent image
    /// contributes a <c>srcset</c> entry using its <c>scaling</c> property.
    /// </summary>
    [HtmlAttributeName("images")]
    public IEnumerable<IPublishedContent> Images { get; set; } = [];

    /// <summary>
    /// The web host environment.
    /// </summary>
    protected IWebHostEnvironment WebHostEnvironment { get; } = webHostEnvironment;

    /// <summary>
    /// Memory cache used to store resolved image dimensions.
    /// </summary>
    protected IMemoryCache MemoryCache { get; } = memoryCache;

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Images == null)
        {
            output.SuppressOutput();
            return;
        }

        var image = Images.FirstOrDefault();

        if (image == null)
        {
            output.SuppressOutput();
            return;
        }

        var altText = image.Value<string>("umbracoAltText");
        altText = String.IsNullOrWhiteSpace(altText) ? String.Empty : altText;

        string srcset = String.Empty;
        foreach (var img in Images.Skip(1))
        {
            var scaling = img.Value<float?>("scaling");

            if (scaling == null) continue;

            srcset += $", {FormatSrcsetEntry(img.Url(), scaling.Value)}";
        }
        srcset = srcset.TrimStart(", ");

        output.TagName = "img";

        output.Attributes.Add("src", image.Url());
        output.Attributes.Add("srcset", srcset);
        output.Attributes.Add("alt", altText);

        var height = image.Value<int>("umbracoHeight");
        var width = image.Value<int>("umbracoWidth");

        if (height == 0 || width == 0)
        {
            if (!WebHostEnvironment.IsDevelopment() && MemoryCache.TryGetValue(image.Url(), out (int Width, int Height) data))
            {
                width = data.Width;
                height = data.Height;
            }
            else
            {
                string path = ToPhysicalPath(WebHostEnvironment.WebRootPath, image.Url());

                if (!File.Exists(path)) return;

                try
                {
                    using var imageFile = Image.Load(path);
                    width = imageFile.Width;
                    height = imageFile.Height;
                    MemoryCache.Set(image.Url(), (width, height));
                }
                catch
                {
                    // The file could not be decoded; emit no dimensions rather than 0x0.
                    return;
                }
            }
        }

        output.Attributes.Add("width", width);
        output.Attributes.Add("height", height);
    }

    /// <summary>
    /// Formats a single <c>srcset</c> entry, rendering the scaling descriptor with the invariant
    /// culture so comma-decimal locales don't corrupt the value.
    /// </summary>
    internal static string FormatSrcsetEntry(string url, float scaling) =>
        $"{url} {scaling.ToString(CultureInfo.InvariantCulture)}x";

    /// <summary>
    /// Maps a (possibly Umbraco <c>~/</c>-rooted) media URL to a physical path under the web root,
    /// stripping any query string and normalising separators for the current platform.
    /// </summary>
    internal static string ToPhysicalPath(string webRootPath, string url)
    {
        string cleanPath = url.Replace('~', '.');

        int queryIndex = cleanPath.IndexOf('?');
        if (queryIndex >= 0)
        {
            cleanPath = cleanPath[..queryIndex];
        }

        cleanPath = cleanPath.Replace('/', Path.DirectorySeparatorChar);

        return Path.Combine(webRootPath, cleanPath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }
}
