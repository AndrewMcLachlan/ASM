using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asm.IO;

namespace Asm.Tests.IO;

public class StringWriterWithEncodingTests
{
    public static readonly List<object[]> encodings = [new Encoding[] { Encoding.UTF8 }, new Encoding[] { Encoding.ASCII }, new Encoding[] { Encoding.Latin1 } ];

    [Theory]
    [MemberData(nameof(encodings))]
    public void EncodingTest(Encoding encoding)
    {
        // Arrange
        var writer = new StringWriterWithEncoding(encoding);
        // Act
        var result = writer.Encoding;
        // Assert
        Assert.Equal(encoding, result);
    }
}
