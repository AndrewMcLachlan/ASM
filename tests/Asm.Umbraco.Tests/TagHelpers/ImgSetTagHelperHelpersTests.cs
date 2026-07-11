using System.Globalization;
using Asm.Umbraco.TagHelpers;

namespace Asm.Umbraco.Tests.TagHelpers;

public class ImgSetTagHelperHelpersTests
{
    /// <summary>
    /// Given the current culture uses a comma decimal separator (de-DE)
    /// When FormatSrcsetEntry formats a fractional density descriptor
    /// Then the density is rendered with a dot separator using the invariant culture ("1.5x")
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FormatSrcsetEntryUsesInvariantCulture()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            // A comma-decimal culture would otherwise render "1,5x", which is invalid srcset syntax.
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            var entry = ImgSetTagHelper.FormatSrcsetEntry("/media/img.jpg", 1.5f);

            Assert.Equal("/media/img.jpg 1.5x", entry);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    /// <summary>
    /// Given a media URL that includes a query string
    /// When ToPhysicalPath converts it to a physical path
    /// Then the query string is removed and the path ends with the media file name
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToPhysicalPathStripsQueryString()
    {
        var path = ImgSetTagHelper.ToPhysicalPath("/wwwroot", "/media/pic.jpg?width=100");

        Assert.DoesNotContain("?", path);
        Assert.EndsWith($"media{Path.DirectorySeparatorChar}pic.jpg", path);
    }

    /// <summary>
    /// Given a root-relative media URL and a web root path
    /// When ToPhysicalPath converts it to a physical path
    /// Then the URL is combined under the web root rather than replacing it
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToPhysicalPathCombinesRootRelativeUrlUnderWebRoot()
    {
        // A rooted media URL must resolve under the web root, not replace it.
        var webRoot = Path.Combine("app", "wwwroot");
        var path = ImgSetTagHelper.ToPhysicalPath(webRoot, "/media/pic.jpg");

        Assert.StartsWith(webRoot, path);
        Assert.EndsWith($"media{Path.DirectorySeparatorChar}pic.jpg", path);
    }

    /// <summary>
    /// Given a URL consisting only of a query string
    /// When ToPhysicalPath converts it to a physical path
    /// Then the query string is stripped from the result
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToPhysicalPathStripsQueryAtStart()
    {
        var path = ImgSetTagHelper.ToPhysicalPath("/wwwroot", "?x=1");

        Assert.DoesNotContain("?", path);
    }
}
