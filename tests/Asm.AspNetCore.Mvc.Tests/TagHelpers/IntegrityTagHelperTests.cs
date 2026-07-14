using Asm.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Asm.AspNetCore.Mvc.Tests.TagHelpers;

[Trait("Category", "Unit")]
public sealed class IntegrityTagHelperTests : IDisposable
{
    private readonly string _webRoot = Directory.CreateTempSubdirectory("asm-integrity-").FullName;

    public void Dispose() => Directory.Delete(_webRoot, recursive: true);

    private LinkTagHelper CreateLinkTagHelper()
    {
        var env = new Mock<IWebHostEnvironment>();
        env.SetupGet(e => e.WebRootPath).Returns(_webRoot);
        env.SetupGet(e => e.EnvironmentName).Returns("Production");

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.Content(It.IsAny<string>())).Returns((string s) => s);

        var factory = new Mock<IUrlHelperFactory>();
        factory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelper.Object);

        return new LinkTagHelper(factory.Object, env.Object, new MemoryCache(new MemoryCacheOptions()), NullLogger<LinkTagHelper>.Instance)
        {
            ViewContext = new ViewContext { HttpContext = new DefaultHttpContext() },
        };
    }

    private static (TagHelperContext Context, TagHelperOutput Output) StylesheetLink(string href, bool includeIntegrity = false)
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("rel", new HtmlString("stylesheet")),
            new TagHelperAttribute("href", href),
        };
        if (includeIntegrity)
        {
            attributes.Add(new TagHelperAttribute("integrity", "sha512-existing"));
        }

        var context = new TagHelperContext(attributes, new Dictionary<object, object>(), "id");
        var output = new TagHelperOutput("link", new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        return (context, output);
    }

    /// <summary>
    /// Given a stylesheet link whose file exists under the web root
    /// When the tag helper processes it
    /// Then a sha512 integrity attribute is added
    /// </summary>
    [Fact]
    public void ProcessAddsSha512IntegrityForExistingFile()
    {
        File.WriteAllText(Path.Combine(_webRoot, "site.css"), "body { color: red; }");
        var (context, output) = StylesheetLink("site.css");

        CreateLinkTagHelper().Process(context, output);

        var integrity = output.Attributes["integrity"];
        Assert.NotNull(integrity);
        Assert.StartsWith("sha512-", integrity.Value?.ToString());
    }

    /// <summary>
    /// Given a stylesheet link that already carries an integrity attribute
    /// When the tag helper processes it
    /// Then it leaves the output untouched
    /// </summary>
    [Fact]
    public void ProcessSkipsWhenIntegrityAlreadyPresent()
    {
        File.WriteAllText(Path.Combine(_webRoot, "site.css"), "body {}");
        var (context, output) = StylesheetLink("site.css", includeIntegrity: true);

        CreateLinkTagHelper().Process(context, output);

        Assert.Empty(output.Attributes);
    }

    /// <summary>
    /// Given a stylesheet link whose file does not exist under the web root
    /// When the tag helper processes it
    /// Then no integrity attribute is added
    /// </summary>
    [Fact]
    public void ProcessAddsNoIntegrityWhenFileMissing()
    {
        var (context, output) = StylesheetLink("missing.css");

        CreateLinkTagHelper().Process(context, output);

        Assert.Null(output.Attributes["integrity"]);
    }

    /// <summary>
    /// Given a link that is not a stylesheet
    /// When the tag helper processes it
    /// Then it is skipped and no integrity attribute is added
    /// </summary>
    [Fact]
    public void ProcessSkipsNonStylesheetLink()
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("rel", new HtmlString("icon")),
            new TagHelperAttribute("href", "favicon.ico"),
        };
        var context = new TagHelperContext(attributes, new Dictionary<object, object>(), "id");
        var output = new TagHelperOutput("link", new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        CreateLinkTagHelper().Process(context, output);

        Assert.Null(output.Attributes["integrity"]);
    }
}
