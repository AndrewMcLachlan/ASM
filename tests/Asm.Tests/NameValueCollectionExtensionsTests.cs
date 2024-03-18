using System.Collections.Specialized;
using Xunit;

namespace Asm.Tests;

public class NameValueCollectionExtensionsTests
{
    [Fact]
    public void GetValueTest()
    {
        NameValueCollection collection = new NameValueCollection();

        collection.Add("Test", "2");

        int value = collection.GetValue("Test", 1);

        Assert.Equal(2, value);

        value = collection.GetValue("Wibble", 5);

        Assert.Equal(5, value);
    }
}
