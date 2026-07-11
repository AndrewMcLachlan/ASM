using System.Text.Json;
using Asm;
using Asm.Drawing;

namespace Asm.Tests;

/// <summary>
/// Focused tests for the Phase 1g correctness fixes in the core Asm library.
/// </summary>
public class CoreFixesTests
{
    /// <summary>
    /// Given two ByteArray instances with the same bytes and endianness.
    /// When they are compared and their hash codes taken.
    /// Then they are equal and produce equal hash codes.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ByteArrayEqualInstancesHaveEqualHashCodes()
    {
        var a = new ByteArray([1, 2, 3, 4], Endian.BigEndian);
        var b = new ByteArray([1, 2, 3, 4], Endian.BigEndian);

        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    /// <summary>
    /// Given two ByteArray instances with the same bytes but different endianness.
    /// When their hash codes are taken.
    /// Then the hash codes differ.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ByteArrayDifferentEndianHaveDifferentHashCodes()
    {
        var big = new ByteArray([1, 2, 3, 4], Endian.BigEndian);
        var little = new ByteArray([1, 2, 3, 4], Endian.LittleEndian);

        Assert.NotEqual(big.GetHashCode(), little.GetHashCode());
    }

    /// <summary>
    /// Given an empty array of nybbles.
    /// When Nybble.ToUInt32 is called.
    /// Then an ArgumentException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NybbleToUInt32EmptyArrayThrows()
    {
        Assert.Throws<ArgumentException>(() => Nybble.ToUInt32([]));
    }

    /// <summary>
    /// Given an array of nine nybbles, more than fit in a UInt32.
    /// When Nybble.ToUInt32 is called.
    /// Then an ArgumentOutOfRangeException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NybbleToUInt32MoreThanEightNybblesThrows()
    {
        var nybbles = Enumerable.Range(0, 9).Select(_ => new Nybble(1)).ToArray();

        Assert.Throws<ArgumentOutOfRangeException>(() => Nybble.ToUInt32(nybbles));
    }

    /// <summary>
    /// Given a JSON string that is not a valid hex colour ("#GGGGGG").
    /// When it is deserialized to a HexColour.
    /// Then a JsonException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void HexColourDeserializeInvalidStringThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<HexColour>("\"#GGGGGG\""));
    }

    /// <summary>
    /// Given a JSON number token ("123") rather than a string.
    /// When it is deserialized to a HexColour.
    /// Then a JsonException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void HexColourDeserializeNumberTokenThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<HexColour>("123"));
    }

    /// <summary>
    /// Given a call to Squish with a fromEnd value larger than the string.
    /// When the out-of-range ArgumentOutOfRangeException is thrown.
    /// Then its ParamName reports the "fromEnd" parameter.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void SquishFromEndOutOfRangeReportsFromEndParameter()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => "abc".Squish(fromEnd: 10));

        Assert.Equal("fromEnd", exception.ParamName);
    }
}
