using Asm.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asm.AspNetCore.Mvc.Tests.TagHelpers;

[Trait("Category", "Unit")]
public class CanonicalTagHelperTests
{
    private static (TagHelperContext Context, TagHelperOutput Output) CreateContextAndOutput()
    {
        var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");
        var output = new TagHelperOutput("canonical", new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        return (context, output);
    }

    private static ViewContext ViewContextFor(string scheme, string host)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = scheme;
        httpContext.Request.Host = new HostString(host);
        return new ViewContext { HttpContext = httpContext };
    }

    /// <summary>
    /// Given a canonical tag helper with a path
    /// When it is processed
    /// Then it emits a rel=canonical link with the absolute href
    /// </summary>
    [Fact]
    public void ProcessEmitsCanonicalLinkTag()
    {
        var (context, output) = CreateContextAndOutput();
        var helper = new CanonicalTagHelper { Path = "about", ViewContext = ViewContextFor("https", "example.com") };

        helper.Process(context, output);

        Assert.Equal("link", output.TagName);
        Assert.Equal("canonical", output.Attributes["rel"].Value);
        Assert.Equal("https://example.com/about", output.Attributes["href"].Value);
    }

    /// <summary>
    /// Given a canonical tag helper with an empty path
    /// When it is processed
    /// Then the trailing slash is trimmed from the href
    /// </summary>
    [Fact]
    public void ProcessTrimsTrailingSlashForRootPath()
    {
        var (context, output) = CreateContextAndOutput();
        var helper = new CanonicalTagHelper { Path = "", ViewContext = ViewContextFor("https", "example.com") };

        helper.Process(context, output);

        Assert.Equal("https://example.com", output.Attributes["href"].Value);
    }
}
