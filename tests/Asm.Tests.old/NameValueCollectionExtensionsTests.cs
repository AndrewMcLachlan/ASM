using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using Asm.Extensions;
using xUnit;

namespace Asm.Tests
{
    public class NameValueCollectionExtensionsTests
    {
        [Fact]
        public void GetValueTest()
        {
            NameValueCollection collection = new NameValueCollection();

            collection.Add("Test", "2");

            int value = collection.GetValue("Test", 1);

            Assert.AreEqual(2, value);

            value = collection.GetValue("Wibble", 5);

            Assert.AreEqual(5, value);
        }
    }
}
