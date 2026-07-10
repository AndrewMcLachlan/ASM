using Asm;

namespace Asm.Tests;

/// <summary>
/// Covers the Phase 3 ExtendedBitArray fixes not exercised by the feature file:
/// multi-byte GetBytes, ICollection.CopyTo, the from-end Range indexer and BigEndian value semantics.
/// </summary>
public class ExtendedBitArrayExtraTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void GetBytes_MultiByte_PacksEveryByte()
    {
        // 16 bits: byte 0 = 5, byte 1 = 1. The old stride bug only wrote byte 0.
        bool[] bits = [true, false, true, false, false, false, false, false, true, false, false, false, false, false, false, false];
        var eba = new ExtendedBitArray(bits, Endian.LittleEndian);

        Assert.Equal([(byte)5, (byte)1], eba.GetBytes());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetBytes_RoundTripsFromBytes()
    {
        var eba = new ExtendedBitArray(new byte[] { 5, 1, 200 }, Endian.LittleEndian);

        Assert.Equal([(byte)5, (byte)1, (byte)200], eba.GetBytes());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CopyTo_Array_DoesNotThrowAndCopies()
    {
        var eba = new ExtendedBitArray([true, false, true, false], Endian.LittleEndian);
        var dest = new bool[4];

        eba.CopyTo(dest, 0);

        Assert.Equal([true, false, true, false], dest);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RangeIndexer_HonoursFromEndIndices()
    {
        var eba = new ExtendedBitArray([true, false, true, false, true, true, false, false], Endian.LittleEndian);

        // ^4..^1 -> indices 4,5,6
        Assert.Equal([true, true, false], eba[^4..^1]);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RangeIndexer_FullFromEndRange()
    {
        var eba = new ExtendedBitArray([true, false, true, false], Endian.LittleEndian);

        Assert.Equal([true, false, true, false], eba[..^0]);
    }

    // BigEndian value semantics: bit 0 is the most-significant bit, so [t,f,t,f] reads as 1010b = 10.
    [Fact]
    [Trait("Category", "Unit")]
    public void BigEndian_ToByte_IsMostSignificantBitFirst()
    {
        var eba = new ExtendedBitArray([true, false, true, false], Endian.BigEndian);

        Assert.Equal((byte)10, eba.ToByte());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void BigEndian_ToSByte_HandlesSignBit()
    {
        // MSB set, rest clear -> -128 in two's complement.
        var eba = new ExtendedBitArray([true, false, false, false, false, false, false, false], Endian.BigEndian);

        Assert.Equal((sbyte)-128, eba.ToSByte());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void LittleEndian_ToInt16_NegativeValue()
    {
        // All 16 bits set -> -1.
        var bits = Enumerable.Repeat(true, 16).ToArray();
        var eba = new ExtendedBitArray(bits, Endian.LittleEndian);

        Assert.Equal((short)-1, eba.ToInt16());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToUnsigned_ArrayLongerThanTarget_TruncatesToLowBits()
    {
        // 16-bit array converted to a byte keeps only the low 8 bits (was OverflowException).
        bool[] bits = [true, false, true, false, false, false, false, false, true, true, true, true, true, true, true, true];
        var eba = new ExtendedBitArray(bits, Endian.LittleEndian);

        Assert.Equal((byte)5, eba.ToByte());
    }
}
