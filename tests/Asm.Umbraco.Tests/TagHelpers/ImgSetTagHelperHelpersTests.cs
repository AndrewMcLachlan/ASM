using System.Globalization;
using Asm.Umbraco.TagHelpers;

namespace Asm.Umbraco.Tests.TagHelpers;

public class ImgSetTagHelperHelpersTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void FormatSrcsetEntry_UsesInvariantCulture()
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

    [Fact]
    [Trait("Category", "Unit")]
    public void ToPhysicalPath_StripsQueryString()
    {
        var path = ImgSetTagHelper.ToPhysicalPath("/wwwroot", "/media/pic.jpg?width=100");

        Assert.DoesNotContain("?", path);
        Assert.EndsWith($"media{Path.DirectorySeparatorChar}pic.jpg", path);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToPhysicalPath_RootRelativeUrl_CombinesUnderWebRoot()
    {
        // A rooted media URL must resolve under the web root, not replace it.
        var webRoot = Path.Combine("app", "wwwroot");
        var path = ImgSetTagHelper.ToPhysicalPath(webRoot, "/media/pic.jpg");

        Assert.StartsWith(webRoot, path);
        Assert.EndsWith($"media{Path.DirectorySeparatorChar}pic.jpg", path);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToPhysicalPath_QueryAtStart_IsStripped()
    {
        var path = ImgSetTagHelper.ToPhysicalPath("/wwwroot", "?x=1");

        Assert.DoesNotContain("?", path);
    }
}
