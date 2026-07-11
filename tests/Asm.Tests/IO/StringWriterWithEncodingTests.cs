using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asm.IO;

namespace Asm.Tests.IO;

public class StringWriterWithEncodingTests
{
    public static readonly List<object[]> encodings = [new Encoding[] { Encoding.UTF8 }, new Encoding[] { Encoding.ASCII }, new Encoding[] { Encoding.Latin1 }];

    /// <summary>
    /// Given a StringWriterWithEncoding constructed with a specific encoding.
    /// When the Encoding property is read.
    /// Then it returns the encoding supplied to the constructor.
    /// </summary>
    [Theory]
    [MemberData(nameof(encodings))]
    public void EncodingReturnsConstructorEncoding(Encoding encoding)
    {
        // Arrange
        var writer = new StringWriterWithEncoding(encoding);
        // Act
        var result = writer.Encoding;
        // Assert
        Assert.Equal(encoding, result);
    }
}
