using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Asm.Umbraco.Tests.Extensions;

public class IPublishedContentExtensionsTests
{
    // -----------------------------------------------------------------------
    // NameAsCssClass
    // -----------------------------------------------------------------------

    /// <summary>
    /// Given published content whose Name contains a space and mixed case
    /// When NameAsCssClass is called
    /// Then a lowercase, hyphen-separated CSS class name is returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClassReturnsLowercaseHyphenSeparatedName()
    {
        var contentMock = new Mock<IPublishedContent>();
        contentMock.Setup(c => c.Name).Returns("Hello World");

        var result = contentMock.Object.NameAsCssClass();

        Assert.Equal("hello-world", result);
    }

    /// <summary>
    /// Given a null IPublishedContent reference
    /// When NameAsCssClass is called
    /// Then null is returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClassReturnsNullWhenContentIsNull()
    {
        IPublishedContent? content = null;
        var result = content.NameAsCssClass();
        Assert.Null(result);
    }

    /// <summary>
    /// Given non-null published content whose Name is null
    /// When NameAsCssClass is called
    /// Then null is returned rather than throwing
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClassReturnsNullWhenNameIsNull()
    {
        var contentMock = new Mock<IPublishedContent>();
        contentMock.Setup(c => c.Name).Returns((string)null!);

        var result = contentMock.Object.NameAsCssClass();

        Assert.Null(result);
    }

    /// <summary>
    /// Given published content whose Name contains multiple spaces
    /// When NameAsCssClass is called
    /// Then each space is replaced with a hyphen in the returned CSS class name
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClassReplacesMultipleSpacesEachWithHyphen()
    {
        var contentMock = new Mock<IPublishedContent>();
        contentMock.Setup(c => c.Name).Returns("One Two Three");

        var result = contentMock.Object.NameAsCssClass();

        Assert.Equal("one-two-three", result);
    }

    /// <summary>
    /// Given published content whose Name is already lowercase but contains a space
    /// When NameAsCssClass is called
    /// Then the name is returned unchanged except that the space is replaced with a hyphen
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NameAsCssClassLeavesAlreadyLowercaseUnchangedExceptSpaces()
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
