using System.Text.Json;
using Asm;
using Asm.Drawing;

namespace Asm.Tests;

/// <summary>
/// Focused tests for the Phase 1g correctness fixes in the core Asm library.
/// </summary>
public class CoreFixesTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ByteArray_EqualInstances_HaveEqualHashCodes()
    {
        var a = new ByteArray([1, 2, 3, 4], Endian.BigEndian);
        var b = new ByteArray([1, 2, 3, 4], Endian.BigEndian);

        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ByteArray_DifferentEndian_HaveDifferentHashCodes()
    {
        var big = new ByteArray([1, 2, 3, 4], Endian.BigEndian);
        var little = new ByteArray([1, 2, 3, 4], Endian.LittleEndian);

        Assert.NotEqual(big.GetHashCode(), little.GetHashCode());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Nybble_ToUInt32_EmptyArray_Throws()
    {
        Assert.Throws<ArgumentException>(() => Nybble.ToUInt32([]));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Nybble_ToUInt32_MoreThanEightNybbles_Throws()
    {
        var nybbles = Enumerable.Range(0, 9).Select(_ => new Nybble(1)).ToArray();

        Assert.Throws<ArgumentOutOfRangeException>(() => Nybble.ToUInt32(nybbles));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void HexColour_DeserializeInvalidString_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<HexColour>("\"#GGGGGG\""));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void HexColour_DeserializeNumberToken_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<HexColour>("123"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Squish_FromEndOutOfRange_ReportsFromEndParameter()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => "abc".Squish(fromEnd: 10));

        Assert.Equal("fromEnd", exception.ParamName);
    }
}
