using Asm;

namespace Asm.Tests;

/// <summary>
/// Covers the Phase 3 ExtendedBitArray fixes not exercised by the feature file:
/// multi-byte GetBytes, ICollection.CopyTo, the from-end Range indexer and BigEndian value semantics.
/// </summary>
public class ExtendedBitArrayExtraTests
{
    /// <summary>
    /// Given a 16-bit little-endian ExtendedBitArray whose two bytes are 5 and 1.
    /// When GetBytes is called.
    /// Then every byte is packed, returning [5, 1] rather than only the first byte.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetBytesMultiBytePacksEveryByte()
    {
        // 16 bits: byte 0 = 5, byte 1 = 1. The old stride bug only wrote byte 0.
        bool[] bits = [true, false, true, false, false, false, false, false, true, false, false, false, false, false, false, false];
        var eba = new ExtendedBitArray(bits, Endian.LittleEndian);

        Assert.Equal([(byte)5, (byte)1], eba.GetBytes());
    }

    /// <summary>
    /// Given an ExtendedBitArray constructed from the bytes [5, 1, 200].
    /// When GetBytes is called.
    /// Then it round-trips the original bytes unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetBytesRoundTripsFromBytes()
    {
        var eba = new ExtendedBitArray(new byte[] { 5, 1, 200 }, Endian.LittleEndian);

        Assert.Equal([(byte)5, (byte)1, (byte)200], eba.GetBytes());
    }

    /// <summary>
    /// Given an ExtendedBitArray of four bits and a destination bool array.
    /// When CopyTo is called with offset 0.
    /// Then it does not throw and copies every bit into the destination.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CopyToArrayDoesNotThrowAndCopies()
    {
        var eba = new ExtendedBitArray([true, false, true, false], Endian.LittleEndian);
        var dest = new bool[4];

        eba.CopyTo(dest, 0);

        Assert.Equal([true, false, true, false], dest);
    }

    /// <summary>
    /// Given an eight-bit ExtendedBitArray.
    /// When it is indexed with a from-end range (^4..^1).
    /// Then it returns the bits at indices 4, 5 and 6.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void RangeIndexerHonoursFromEndIndices()
    {
        var eba = new ExtendedBitArray([true, false, true, false, true, true, false, false], Endian.LittleEndian);

        // ^4..^1 -> indices 4,5,6
        Assert.Equal([true, true, false], eba[^4..^1]);
    }

    /// <summary>
    /// Given a four-bit ExtendedBitArray.
    /// When it is indexed with a full from-end range (..^0).
    /// Then it returns all four bits unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void RangeIndexerFullFromEndRange()
    {
        var eba = new ExtendedBitArray([true, false, true, false], Endian.LittleEndian);

        Assert.Equal([true, false, true, false], eba[..^0]);
    }

    // BigEndian value semantics: bit 0 is the most-significant bit, so [t,f,t,f] reads as 1010b = 10.
    /// <summary>
    /// Given a big-endian ExtendedBitArray of bits [true, false, true, false].
    /// When ToByte is called.
    /// Then bit 0 is treated as the most-significant bit, yielding 1010b = 10.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void BigEndianToByteIsMostSignificantBitFirst()
    {
        var eba = new ExtendedBitArray([true, false, true, false], Endian.BigEndian);

        Assert.Equal((byte)10, eba.ToByte());
    }

    /// <summary>
    /// Given a big-endian ExtendedBitArray with only the most-significant bit set.
    /// When ToSByte is called.
    /// Then the sign bit is honoured, yielding -128 in two's complement.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void BigEndianToSByteHandlesSignBit()
    {
        // MSB set, rest clear -> -128 in two's complement.
        var eba = new ExtendedBitArray([true, false, false, false, false, false, false, false], Endian.BigEndian);

        Assert.Equal((sbyte)-128, eba.ToSByte());
    }

    /// <summary>
    /// Given a little-endian ExtendedBitArray with all 16 bits set.
    /// When ToInt16 is called.
    /// Then it returns -1.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void LittleEndianToInt16NegativeValue()
    {
        // All 16 bits set -> -1.
        var bits = Enumerable.Repeat(true, 16).ToArray();
        var eba = new ExtendedBitArray(bits, Endian.LittleEndian);

        Assert.Equal((short)-1, eba.ToInt16());
    }

    /// <summary>
    /// Given a 16-bit little-endian ExtendedBitArray whose low byte is 5.
    /// When ToByte converts it to an 8-bit target.
    /// Then only the low 8 bits are kept, yielding 5 instead of throwing an OverflowException.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToUnsignedArrayLongerThanTargetTruncatesToLowBits()
    {
        // 16-bit array converted to a byte keeps only the low 8 bits (was OverflowException).
        bool[] bits = [true, false, true, false, false, false, false, false, true, true, true, true, true, true, true, true];
        var eba = new ExtendedBitArray(bits, Endian.LittleEndian);

        Assert.Equal((byte)5, eba.ToByte());
    }
}
