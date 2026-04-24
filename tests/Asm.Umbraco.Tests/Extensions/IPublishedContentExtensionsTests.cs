using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Asm.Umbraco.Tests.Extensions;

public class IPublishedContentExtensionsTests
{
    // -----------------------------------------------------------------------
    // NameAsCssClass
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClass_ReturnsLowercaseHyphenSeparatedName()
    {
        var contentMock = new Mock<IPublishedContent>();
        contentMock.Setup(c => c.Name).Returns("Hello World");

        var result = contentMock.Object.NameAsCssClass();

        Assert.Equal("hello-world", result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClass_WhenNullContent_ReturnsNull()
    {
        IPublishedContent content = null;
        var result = content.NameAsCssClass();
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClass_MultipleSpaces_ReplacesEachWithHyphen()
    {
        var contentMock = new Mock<IPublishedContent>();
        contentMock.Setup(c => c.Name).Returns("One Two Three");

        var result = contentMock.Object.NameAsCssClass();

        Assert.Equal("one-two-three", result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClass_AlreadyLowercase_ReturnsUnchangedExceptSpaces()
    {
        var contentMock = new Mock<IPublishedContent>();
        contentMock.Setup(c => c.Name).Returns("already lower");

        var result = contentMock.Object.NameAsCssClass();

        Assert.Equal("already-lower", result);
    }

    // -----------------------------------------------------------------------
    // ValueOr<T> and ValueAnd — SKIPPED
    //
    // Both ValueOr<T> and ValueAnd call model.HasValue(alias) where model is
    // IPublishedContent. This resolves to FriendlyPublishedContentExtensions.HasValue
    // (from Umbraco.Cms.Web.Common), which initialises via a static type initialiser
    // that calls GetRequiredService<IPublishedValueFallback>() against Umbraco's
    // static service provider (Current). In a unit test environment Umbraco's
    // composition is never set up, so the static constructor throws:
    //   "Value cannot be null (Parameter 'provider')"
    //
    // This failure occurs even on the false-branch path (HasValue returns false →
    // return fallback), because FriendlyPublishedContentExtensions initialises
    // eagerly at first use of any member. There is no way to test these methods
    // in isolation without bootstrapping the full Umbraco composition.
    //
    // Coverage for ValueOr and ValueAnd should be provided by integration or
    // acceptance tests that run inside a fully composed Umbraco host.
    // -----------------------------------------------------------------------
}
