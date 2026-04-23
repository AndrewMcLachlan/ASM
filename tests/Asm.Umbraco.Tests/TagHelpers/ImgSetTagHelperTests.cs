using Asm.Umbraco.TagHelpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Asm.Umbraco.Tests.TagHelpers;

// NOTE: Tests that exercise the full Process() path (non-empty image list) are
// not included here because image.Url() resolves via FriendlyPublishedContentExtensions
// which requires IPublishedUrlProvider from Umbraco's composition/service locator.
// Bootstrapping the full Umbraco composition in a unit test is disproportionate
// effort for the value gained. Those paths are better covered by integration tests.
//
// What IS covered: the short-circuit paths (null/empty images) that suppress output
// without touching the Umbraco extension method chain.

public class ImgSetTagHelperTests
{
    private static TagHelperContext CreateContext() =>
        new(
            tagName: "imgset",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

    private static TagHelperOutput CreateOutput() =>
        new(
            tagName: "imgset",
            attributes: new TagHelperAttributeList(),
            getChildContentAsync: (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

    private ImgSetTagHelper CreateTagHelper(IEnumerable<IPublishedContent> images = null)
    {
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.EnvironmentName).Returns("Production");
        envMock.Setup(e => e.WebRootPath).Returns("/var/www");

        var cacheMock = new Mock<IMemoryCache>();

        var helper = new ImgSetTagHelper(envMock.Object, cacheMock.Object)
        {
            ViewContext = new ViewContext(),
            Images = images ?? []
        };

        return helper;
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Process_WhenImagesIsNull_SuppressesOutput()
    {
        var helper = CreateTagHelper();
        helper.Images = null;

        var output = CreateOutput();
        helper.Process(CreateContext(), output);

        // SuppressOutput() sets TagName to null
        Assert.Null(output.TagName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Process_WhenImagesIsEmpty_SuppressesOutput()
    {
        var helper = CreateTagHelper(images: []);

        var output = CreateOutput();
        helper.Process(CreateContext(), output);

        // An empty Images enumerable means FirstOrDefault() returns null, triggering SuppressOutput()
        Assert.Null(output.TagName);
    }
}
