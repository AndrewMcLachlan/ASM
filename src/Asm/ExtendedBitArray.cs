#pragma warning disable 8618 // Non-nullable field is uninitialized. To be fixed on review.
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Asm;

/// <summary>
/// An array of bits in either little endian or big endian format.
/// </summary>
/// <remarks>
/// This code has not been touched in years and needs unit tests.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), Serializable]
[CLSCompliant(false)]
public sealed class ExtendedBitArray : ICollection, IEnumerable<bool>, ICloneable
{
    #region Fields
    private BitArray _bitArray;
    private readonly Endian _endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the value of the bit at the given position.
    /// </summary>
    public bool this[int index] => _bitArray[index];

    /// <summary>
    /// Gets the range of bits from the given start to end.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool[] this[Range range]
    {
        get
        {
            bool[] temp = new bool[range.End.Value - range.Start.Value];
            for (int i = range.Start.Value, j = 0; i < range.End.Value; i++, j++)
            {
                temp[j] = this[i];
            }
            return temp;
        }
    }

    /// <summary>
    /// Gets the endianness of the array.
    /// </summary>
    public Endian Endian => _endian;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="values">The bit array to extend.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(BitArray values, Endian endian)
    {
        _endian = endian;
        _bitArray = values;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="values">An array of boolean values representing individual bits.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(bool[] values, Endian endian)
    {
        _endian = endian;
        _bitArray = GetBits(values, endian);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="values">An array of boolean values representing individual bits.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(ReadOnlySpan<bool> values, Endian endian)
    {
        _endian = endian;
        _bitArray = GetBits(values, endian);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="values">A byte array to convert.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(byte[] values, Endian endian)
    {
        _endian = endian;
        GetBits(values);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="values">A byte array to convert.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(ReadOnlySpan<byte> values, Endian endian)
    {
        _endian = endian;
        GetBits(values.ToArray());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="value">A byte value to convert.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(byte value, Endian endian)
    {
        _endian = endian;
        GetBits([value]);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="value">A signed-byte value to convert.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(sbyte value, Endian endian)
    {
        _endian = endian;
        GetBits([Convert.ToByte(value)]);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="value">A 16 bit integer value to convert.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(short value, Endian endian)
    {
        _endian = endian;
        GetBits(BitConverter.GetBytes(value));
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="value">An integer value to convert.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(int value, Endian endian)
    {
        _endian = endian;
        GetBits(BitConverter.GetBytes(value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
    /// </summary>
    /// <param name="value">A 64 bit integer value to convert.</param>
    /// <param name="endian">The endianness of the array.</param>
    public ExtendedBitArray(long value, Endian endian)
    {
        _endian = endian;
        GetBits(BitConverter.GetBytes(value));
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns the byte representation of the array.
    /// </summary>
    /// <returns>A byte array.</returns>
    public byte[] GetBytes()
    {
        byte[] temp = new byte[(int)Math.Ceiling(_bitArray.Length / 8d)];

        for (int i = 0; i < temp.Length; i+=8)
        {
            temp[i] = (byte)ToUnsigned(1, i);
        }

        return temp;
    }

    /// <summary>
    /// Creates a copy of part of the array.
    /// </summary>
    /// <param name="start">Where the new array will start from.</param>
    /// <param name="length">The length of the new array.</param>
    /// <returns>A new ExtendedBitArray.</returns>
    public ExtendedBitArray Copy(int start, int length)
    {
        int end = start + length;

        if (end - 1 > Count) throw new ArgumentOutOfRangeException(nameof(start));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, Count);

        ReadOnlySpan<bool> temp = this[start..end];

        return new ExtendedBitArray(temp, Endian);
    }

    /// <summary>
    /// Creates a copy of part of the array.
    /// </summary>
    /// <param name="array">The array to copy to.</param>
    /// <param name="index">The index where copying begins.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="index"/> is less than 0 or greater than the length of <paramref name="array"/>.</exception>
    public void CopyTo(ExtendedBitArray array, int index)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (index < 0) throw new ArgumentException("index cannot be less than 0.");
        if (index > array._bitArray.Length - 1) throw new ArgumentException("index cannot be greater than the length of the array.");

        bool[] temp = new bool[array._bitArray.Length];
        array._bitArray.CopyTo(temp, 0);
        _bitArray.CopyTo(temp, index);
        array._bitArray = new BitArray(temp);
    }

    /// <summary>
    /// Returns the signed 8 bit integer representation of the array.
    /// </summary>
    /// <returns>An signed byte.</returns>
    /// <remarks>
    /// If the array is greater than 8 bits long, only the first 8 bits will be used.
    /// </remarks>
    public sbyte ToSByte() => Convert.ToSByte(ToSigned(1));

    /// <summary>
    /// Returns the signed 16 bit integer representation of the array.
    /// </summary>
    /// <returns>An signed short.</returns>
    /// <remarks>
    /// If the array is greater than 16 bits long, only the first 16 bits will be used.
    /// </remarks>
    public short ToInt16() => Convert.ToInt16(ToSigned(2));

    /// <summary>
    /// Returns the signed 32 bit integer representation of the array.
    /// </summary>
    /// <returns>An signed byte.</returns>
    /// <remarks>
    /// If the array is greater than 32 bits long, only the first 32 bits will be used.
    /// </remarks>
    public int ToInt32() => Convert.ToInt32(ToSigned(4));

    /// <summary>
    /// Returns the signed 64 bit integer representation of the array.
    /// </summary>
    /// <returns>An signed byte.</returns>
    /// <remarks>
    /// If the array is greater than 64 bits long, only the first 64 bits will be used.
    /// </remarks>
    public long ToInt64() => Convert.ToInt64(ToSigned(8));

    /// <summary>
    /// Returns the unsigned 8 bit integer representation of the array.
    /// </summary>
    /// <returns>An unsigned byte.</returns>
    /// <remarks>
    /// If the array is greater than 8 bits long, only the first 8 bits will be used.
    /// </remarks>
    public byte ToByte() => Convert.ToByte(ToUnsigned(1));

    /// <summary>
    /// Returns the unsigned 16 bit integer representation of the array.
    /// </summary>
    /// <returns>An unsigned 16 bit integer.</returns>
    /// <remarks>
    /// If the array is greater than 16 bits long, only the first 16 bits will be used.
    /// </remarks>
    public ushort ToUInt16() => Convert.ToUInt16(ToUnsigned(2));

    /// <summary>
    /// Returns the unsigned 32 bit integer representation of the array.
    /// </summary>
    /// <returns>An unsigned 32 bit integer.</returns>
    /// <remarks>
    /// If the array is greater than 32 bits long, only the first 32 bits will be used.
    /// </remarks>
    public uint ToUInt32() => Convert.ToUInt32(ToUnsigned(4));

    /// <summary>
    /// Returns the unsigned 64 bit integer representation of the array.
    /// </summary>
    /// <returns>An unsigned 64 bit integer.</returns>
    /// <remarks>
    /// If the array is greater than 64 bits long, only the first 64 bits will be used.
    /// </remarks>
    public ulong ToUInt64() => Convert.ToUInt64(ToUnsigned(8));

    /// <summary>
    /// Returns the string representation of the bits in the array.
    /// </summary>
    /// <returns>A string of binary digits.</returns>
    public override string ToString()
    {
        StringBuilder temp = new();

        foreach (bool b in this)
        {
            temp.Append(Convert.ToInt16(b));
        }

        return temp.ToString();
    }
    #endregion

    #region Private Methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetBits(byte[] bytes)
    {
        _bitArray = new(bytes);
    }

    private static BitArray GetBits(ReadOnlySpan<bool> values, Endian endian)
    {
        BitArray temp = new(values.Length);

        switch ((endian == Endian.LittleEndian) && BitConverter.IsLittleEndian)
        {
            case false:
                for (int i = values.Length - 1, k = 0; i >= 0; i--, k++)
                {
                    temp[k] = values[i];
                }
                break;
            case true:
                for (int i = 0; i < values.Length; i++)
                {
                    temp[i] = values[i];
                }
                break;
        }

        return temp;
    }

    private long ToSigned(int bytesToRead)
    {
        long temp = 0;
        switch (_endian)
        {
            case Endian.BigEndian:
                switch ((bool)this[0])
                {
                    //Negative
                    case true:
                        for (int i = 0, j = Count - 2; i < Count - 1 && i < (bytesToRead * 8) - 1; i++, j--)
                        {
                            if (!this[i])
                            {
                                temp += (long)Math.Pow(2, j);
                            }
                        }
                        temp++;
                        temp = -temp;
                        break;
                    //Positive
                    case false:
                        for (int i = 0, j = Count - 2; i < Count - 1 && i < (bytesToRead * 8) - 1; i++, j--)
                        {
                            if (this[i])
                            {
                                temp += (long)Math.Pow(2, j);
                            }
                        }
                        break;
                }
                break;
            case Endian.LittleEndian:
                switch ((bool)this[Count - 1])
                {
                    case true:
                        for (int i = 0; i < Count - 1; i++)
                        {
                            if (!this[i])
                            {
                                temp += (long)Math.Pow(2, i);
                            }
                        }
                        temp++;
                        temp = -temp;
                        break;
                    case false:
                        for (int i = 0; i < Count - 1; i++)
                        {
                            if (this[i])
                            {
                                temp += (long)Math.Pow(2, i);
                            }
                        }
                        break;
                }
                break;
        }
        return temp;
    }

    private ulong ToUnsigned(int bytesToRead, int start = 0)
    {
        ulong temp = 0;
        switch (_endian)
        {
            case Endian.BigEndian:
                for (int i = start, j = Count - 1; i < Count && i < bytesToRead * 8; i++, j--)
                {
                    if (this[i])
                    {
                        temp += (ulong)Math.Pow(2, j);
                    }
                }
                break;
            case Endian.LittleEndian:
                for (int i = start; i < Count; i++)
                {
                    if (this[i])
                    {
                        temp += (ulong)Math.Pow(2, i);
                    }
                }
                break;
        }
        return temp;
    }
    #endregion

    #region ICollection Members
    /// <summary>
    /// Copies the elements of the <see cref="ExtendedBitArray"/> to an <see cref="System.Array"/>,
    /// starting at a particular <see cref="System.Array"/> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional System.Array that is the destination of the elements
    /// copied from <see cref="ExtendedBitArray"/>. The <see cref="System.Array"/> must have zero-based
    /// indexing.
    /// </param>
    /// <param name="index">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="array"/> is multidimensional.
    /// -or-
    /// The number of elements in the source <see cref="ExtendedBitArray"/>
    /// is greater than the available space from <paramref name="index"/> to the end of the destination
    /// array.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// The type of the source <see cref="ExtendedBitArray"/> cannot be cast automatically
    /// to the type of the destination array.
    /// </exception>
    public void CopyTo(Array array, int index)
    {
        _bitArray.CopyTo(array, index);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="ExtendedBitArray"/>.
    /// </summary>
    /// <returns>
    /// The number of elements contained in the <see cref="ExtendedBitArray"/>.
    /// </returns>
    public int Count
    {
        get { return _bitArray.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="ExtendedBitArray"/>
    /// is synchronised (thread safe).
    /// </summary>
    /// <returns>
    /// true if access to the <see cref="ExtendedBitArray"/> is synchronised (thread
    /// safe); otherwise, false.
    /// </returns>
    public bool IsSynchronized
    {
        get { return _bitArray.IsSynchronized; }
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="ExtendedBitArray"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="ExtendedBitArray"/>.
    /// </returns>
    public object SyncRoot
    {
        get { return _bitArray.SyncRoot; }
    }
    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="ExtendedBitArray"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="System.Collections.IEnumerator"/> for the entire <see cref="ExtendedBitArray"/>.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _bitArray.GetEnumerator();
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Clones this <see cref="ExtendedBitArray"/>.
    /// </summary>
    /// <returns>A copy fo this <see cref="ExtendedBitArray"/>.</returns>
    public object Clone()
    {
        return new ExtendedBitArray((BitArray)_bitArray.Clone(), _endian);
    }

    IEnumerator<bool> IEnumerable<bool>.GetEnumerator()
    {
        var enumerator = _bitArray.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return (bool)enumerator.Current;
        }
    }
    #endregion
}
#pragma warning restore 8618