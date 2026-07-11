using System;
using System.Collections;
using System.Text;

namespace Asm;

/// <summary>
/// Representation of a half byte or 4 bits.
/// </summary>
[Serializable]
[CLSCompliant(false)]
public readonly struct Nybble
{
    #region Fields
    private readonly byte _byteValue;
    /// <summary>
    /// Represents the smallest possible value of a Nybble. This field is constant.
    /// </summary>
    /// <remarks>
    /// The value of this constant is 0;
    /// </remarks>
    public const byte MinValue = 0;
    /// <summary>
    /// Represents the largest possible value of a Nybble. This field is constant.
    /// </summary>
    /// <remarks>
    /// The value of this constant is 15;
    /// </remarks>
    public const byte MaxValue = 15;
    #endregion

    #region Properties
    /// <summary>
    /// The byte representation of the nybble's value.
    /// </summary>
    public readonly byte ByteValue => _byteValue;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Nybble"/> class.
    /// </summary>
    /// <param name="value">The nybble's value.</param>
    public Nybble(byte value)
    {
        if (value > 15)
        {
            throw new OverflowException();
        }

        _byteValue = value;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Convert the given nybble array to an unsigned 32 bit integer.
    /// </summary>
    /// <param name="value">The array of nybbles to convert.</param>
    /// <returns>An unsigned 32 bit integer.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static uint ToUInt32(Nybble[] value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Length == 0) throw new ArgumentException("At least one nybble is required.", nameof(value));
        if (value.Length > 8) throw new ArgumentOutOfRangeException(nameof(value), "A 32-bit unsigned integer holds at most 8 nybbles.");

        uint temp = value[0].ByteValue;

        for (int i = 1; i < value.Length; i++)
        {
            temp <<= 4;
            temp |= value[i].ByteValue;
        }

        return temp;
    }

    /// <summary>
    /// Adds two nybbles together arithmetically.
    /// </summary>
    /// <param name="a">The first nybble.</param>
    /// <param name="b">The second nybble.</param>
    /// <returns>
    /// A <see cref="Nybble"/> holding the arithmetic sum. The result wraps modulo 16 (that is, only the
    /// low four bits are kept), so <c>15 + 15</c> yields <c>14</c>, mirroring how a 4-bit register overflows.
    /// To concatenate two nybbles into a byte (the pre-v4 behaviour of this operator) use <see cref="Combine"/>.
    /// </returns>
    public static Nybble operator +(Nybble a, Nybble b) => Add(a, b);

    /// <summary>
    /// Adds a nybble to an unsigned integer arithmetically.
    /// </summary>
    /// <param name="a">The unsigned integer to add to.</param>
    /// <param name="b">The nybble to add.</param>
    /// <returns>
    /// The arithmetic sum as a <see cref="uint"/>. The result wraps in the usual unchecked <see cref="uint"/>
    /// manner on overflow. To append the nybble as an extra hexadecimal digit (the pre-v4 behaviour of this
    /// operator) use <see cref="Append(uint, Nybble)"/>.
    /// </returns>
    public static uint operator +(uint a, Nybble b) => Add(a, b);

    /// <summary>
    /// Adds a nybble to a byte arithmetically.
    /// </summary>
    /// <param name="a">The byte to add to.</param>
    /// <param name="b">The nybble to add.</param>
    /// <returns>
    /// The arithmetic sum as an <see cref="int"/> (matching C# numeric promotion for <see cref="byte"/> addition).
    /// To append the nybble as an extra hexadecimal digit (the pre-v4 behaviour of this operator) use
    /// <see cref="Append(byte, Nybble)"/>.
    /// </returns>
    public static int operator +(byte a, Nybble b) => Add(a, b);

    /// <summary>
    /// Adds two nybbles together arithmetically, wrapping modulo 16.
    /// </summary>
    /// <param name="a">The first nybble.</param>
    /// <param name="b">The second nybble.</param>
    /// <returns>The arithmetic sum as a <see cref="Nybble"/>; values above 15 wrap modulo 16.</returns>
    public static Nybble Add(Nybble a, Nybble b) => new((byte)((a.ByteValue + b.ByteValue) & MaxValue));

    /// <summary>
    /// Adds a nybble to an unsigned integer arithmetically.
    /// </summary>
    /// <param name="a">The unsigned integer to add to.</param>
    /// <param name="b">The nybble to add.</param>
    /// <returns>The arithmetic sum as a <see cref="uint"/>.</returns>
    public static uint Add(uint a, Nybble b) => a + (uint)b.ByteValue;

    /// <summary>
    /// Adds a nybble to a byte arithmetically.
    /// </summary>
    /// <param name="a">The byte to add to.</param>
    /// <param name="b">The nybble to add.</param>
    /// <returns>The arithmetic sum as an <see cref="int"/>.</returns>
    public static int Add(byte a, Nybble b) => a + b.ByteValue;

    /// <summary>
    /// Combines two nybbles into a single byte, placing <paramref name="high"/> in the upper four bits
    /// and <paramref name="low"/> in the lower four bits.
    /// </summary>
    /// <param name="high">The nybble to place in the most-significant four bits.</param>
    /// <param name="low">The nybble to place in the least-significant four bits.</param>
    /// <returns>The combined byte. For example, <c>Combine(0x5, 0x3)</c> returns <c>0x53</c> (83).</returns>
    /// <remarks>
    /// This is the concatenation behaviour that the <c>+</c> operator performed prior to v4. Use this method
    /// where you previously relied on <c>nybbleA + nybbleB</c> producing a combined byte.
    /// </remarks>
    public static byte Combine(Nybble high, Nybble low) => (byte)((high.ByteValue << 4) | low.ByteValue);

    /// <summary>
    /// Appends a nybble to an unsigned integer as an extra least-significant hexadecimal digit
    /// (shifts <paramref name="value"/> left four bits and stores <paramref name="nybble"/> in the low four bits).
    /// </summary>
    /// <param name="value">The value to shift left and append to.</param>
    /// <param name="nybble">The nybble to place in the least-significant four bits.</param>
    /// <returns>The combined value as a <see cref="ulong"/>. For example, <c>Append(0x5u, 0x3)</c> returns <c>0x53</c> (83).</returns>
    /// <remarks>
    /// This is the concatenation behaviour that the <c>uint + Nybble</c> operator performed prior to v4.
    /// </remarks>
    public static ulong Append(uint value, Nybble nybble) => ((ulong)value << 4) | nybble.ByteValue;

    /// <summary>
    /// Appends a nybble to a byte as an extra least-significant hexadecimal digit
    /// (shifts <paramref name="value"/> left four bits and stores <paramref name="nybble"/> in the low four bits).
    /// </summary>
    /// <param name="value">The value to shift left and append to.</param>
    /// <param name="nybble">The nybble to place in the least-significant four bits.</param>
    /// <returns>The combined value as an <see cref="int"/>. For example, <c>Append((byte)0x5, 0x3)</c> returns <c>0x53</c> (83).</returns>
    /// <remarks>
    /// This is the concatenation behaviour that the <c>byte + Nybble</c> operator performed prior to v4.
    /// </remarks>
    public static int Append(byte value, Nybble nybble) => (value << 4) | nybble.ByteValue;

    /// <summary>
    /// Checks equality a given nybble against this one.
    /// </summary>
    /// <param name="obj">The nybble to check against.</param>
    /// <returns>Whether the nybbles are equal.</returns>
    public override readonly bool Equals(object? obj)
    {
        if (obj == null || obj is not Nybble nybble) return false;

        return nybble._byteValue == this._byteValue;
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>A hash code.</returns>
    public override readonly int GetHashCode() => _byteValue.GetHashCode();

    /// <summary>
    /// Checks equality of two nybbles.
    /// </summary>
    /// <param name="a">The first nybble.</param>
    /// <param name="b">The second nybble.</param>
    /// <returns>Whether the nybbles are equal.</returns>
    public static bool operator ==(Nybble a, Nybble b)
    {
        return a.Equals(b);
    }

    /// <summary>
    /// Checks inequality of two nybbles.
    /// </summary>
    /// <param name="a">The first nybble.</param>
    /// <param name="b">The second nybble.</param>
    /// <returns>Whether the nybbles are not equal.</returns>
    public static bool operator !=(Nybble a, Nybble b)
    {
        return !a.Equals(b);
    }

    /// <summary>
    /// Converts the given value to an array of eight nybbles, most-significant nybble first
    /// (consistent with <see cref="ToNybbles(byte[])"/>).
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>An array of eight nybbles.</returns>
    public static Nybble[] ToNybbles(int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return ToNybbles(bytes);
    }

    /// <summary>
    /// Converts a byte to nybbles.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>An array of nybbles.</returns>
    public static Nybble[] ToNybbles(byte value)
    {
        BitArray bLE = new(new byte[] { value });
        BitArray b = new(8);

        //Convert little-endian to big-endian
        for (int i = 0, j = 7; i < 8; i++, j--)
        {
            b[i] = bLE[j];
        }

        byte b1, b2;
        b1 = Convert.ToByte(b[0]);
        b1 <<= 1;
        b1 |= Convert.ToByte(b[1]);
        b1 <<= 1;
        b1 |= Convert.ToByte(b[2]);
        b1 <<= 1;
        b1 |= Convert.ToByte(b[3]);

        b2 = Convert.ToByte(b[4]);
        b2 <<= 1;
        b2 |= Convert.ToByte(b[5]);
        b2 <<= 1;
        b2 |= Convert.ToByte(b[6]);
        b2 <<= 1;
        b2 |= Convert.ToByte(b[7]);

        Nybble n1 = new(b1);
        Nybble n2 = new(b2);

        return [n1, n2];
    }

    /// <summary>
    /// Converts an array of bytes to nybbles.
    /// </summary>
    /// <param name="value">The bytes to convert.</param>
    /// <returns>An array of nybbles.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static Nybble[] ToNybbles(byte[] value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Nybble[] n = new Nybble[value.Length * 2];

        for (int i = 0, j = 0; i < value.Length; i++, j += 2)
        {
            Nybble[] temp = Nybble.ToNybbles(value[i]);
            n[j] = temp[0];
            n[j + 1] = temp[1];
        }

        return n;
    }
    #endregion

}
