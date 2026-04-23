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

            srcset += $", {img.Url()} {scaling}x";
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
                string cleanPath = image.Url().Replace('~', '.');
                cleanPath = cleanPath[..(cleanPath.IndexOf('?') > 0 ? cleanPath.IndexOf('?') : cleanPath.Length)];
                cleanPath = cleanPath.Replace('/', Path.DirectorySeparatorChar);

                string path = Path.Combine(WebHostEnvironment.WebRootPath, cleanPath.TrimStart('\\'));

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
                    // Do nothing
                }
            }
        }

        output.Attributes.Add("width", width);
        output.Attributes.Add("height", height);
    }
}
