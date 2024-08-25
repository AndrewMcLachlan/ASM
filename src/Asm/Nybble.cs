using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        if (value > 15 || value < 0)
        {
            throw new OverflowException();
        }
        else
        {
            _byteValue = value;
        }
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

        uint temp;

        temp = value[0].ByteValue;

        if (value.Length <= 8)
        {
            for (int i = 1; i < value.Length; i++)
            {
                temp <<= 4;
                temp |= value[i].ByteValue;
            }
        }
        return temp;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static byte operator +(Nybble a, Nybble b)
    {
        return Add(a, b);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ulong operator +(uint a, Nybble b)
    {
        return Add(a, b);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int operator +(byte a, Nybble b)
    {
        return Add(a, b);
    }

    /// <summary>
    /// Adds two nybbles together.
    /// </summary>
    /// <param name="a">The first nybble.</param>
    /// <param name="b">The second nybble.</param>
    /// <returns>the sum of the two nybbles.</returns>
    public static byte Add(Nybble a, Nybble b)
    {
        byte aa = a.ByteValue;
        byte bb = b.ByteValue;

        aa <<= 4;

        return Convert.ToByte(aa | bb);
    }

    /// <summary>
    /// Adds a nybble to an integer.
    /// </summary>
    /// <param name="a">The integer to add to.</param>
    /// <param name="b">The nybble to add.</param>
    /// <returns>the sum of the two numbers.</returns>
    public static ulong Add(uint a, Nybble b)
    {
        long aa = a;
        byte bb = b.ByteValue;

        aa <<= 4;

        long cc = aa | bb;

        return Convert.ToUInt64(cc);
    }

    /// <summary>
    /// Adds a nybble to a byte.
    /// </summary>
    /// <param name="a">The byte to add to.</param>
    /// <param name="b">The nybble to add.</param>
    /// <returns>the sum of the two numbers.</returns>
    public static int Add(byte a, Nybble b)
    {
        int aa = a;
        byte bb = b.ByteValue;

        aa <<= 4;

        return aa | bb;
    }

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
    /// Converts the given value to an array of nybbles.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>An array of nybbles.</returns>
    public static Nybble[] ToNybbles(int value)
    {
        BitVector32 bitVector32 = new(value);
        return ToNybbles(bitVector32);
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

    #region Private Methods
    private static Nybble[] ToNybbles(BitVector32 bits)
    {
        Nybble[] nybbles = new Nybble[8];
        for (int i = 0; i < 8; i += 4)
        {
            byte b = Convert.ToByte(bits[1 << i + 3]);
            b <<= 1;
            b |= Convert.ToByte(bits[1 << i + 2]);
            b <<= 1;
            b |= Convert.ToByte(bits[1 << i + 1]);
            b <<= 1;
            b |= Convert.ToByte(bits[1 << i]);
            Nybble n = new(b);
            nybbles[i / 4] = n;
        }

        return nybbles;
    }
    #endregion
}
