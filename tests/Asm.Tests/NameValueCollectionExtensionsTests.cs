using System.Collections.Specialized;
using Xunit;

namespace Asm.Tests;

public class NameValueCollectionExtensionsTests
{
    /// <summary>
    /// Given a NameValueCollection containing a key with a parseable value and a missing key.
    /// When GetValue is called with a default for each.
    /// Then the present key returns its parsed value and the missing key returns the supplied default.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetValueReturnsParsedValueOrDefault()
    {
        NameValueCollection collection = new NameValueCollection();

        collection.Add("Test", "2");

        int value = collection.GetValue("Test", 1);

        Assert.Equal(2, value);

        value = collection.GetValue("Wibble", 5);

        Assert.Equal(5, value);
    }
}
