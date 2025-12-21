namespace Asm;

/// <summary>
/// An array of bytes.
/// </summary>
/// <remarks>
/// This class handles conversion of byte arrays into larger integer types using either big or little endianness.
/// This has not been touched in years and needs unit tests.
/// </remarks>
[Serializable]
[CLSCompliant(false)]
public readonly struct ByteArray
{
    private delegate T Converter<T>(ReadOnlySpan<byte> span);

    #region Fields
    private readonly byte[] _bytes;
    private readonly static Endian SystemEndian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
    #endregion

    #region Properties
    /// <summary>
    /// Returns a byte from the array.
    /// </summary>
    public readonly byte this[int index]
    {
        get => _bytes[index];
        set => _bytes[index] = value;
    }

    /// <summary>
    /// The ordinary array representation of the bytes
    /// </summary>
    public readonly byte[] GetBytes() => _bytes;

    /// <summary>
    /// The Endianness of the array.
    /// </summary>
    public readonly Endian Endian { get; } // set; } = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArray"/> class.
    /// </summary>
    /// <param name="size">This number of elements in the array.</param>
    public ByteArray(int size)
    {
        _bytes = new byte[size];
        Endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArray"/> class.
    /// </summary>
    /// <param name="size">This number of elements in the array.</param>
    /// <param name="type">The <see cref="Endian"/>ness of the array.</param>
    public ByteArray(int size, Endian type)
    {
        _bytes = new byte[size];
        Endian = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArray"/> class.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    public ByteArray(byte[] value)
    {
        this._bytes = value;
        Endian = Endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArray"/> class.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="type">The <see cref="Endian"/>ness of the array.</param>
    public ByteArray(byte[] value, Endian type)
    {
        this._bytes = value;
        Endian = type;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Creates a copy of the array.
    /// </summary>
    /// <param name="start">The element from which to start the copy.</param>
    /// <param name="length">The number of elements to copy.</param>
    /// <returns>A new ByteArray.</returns>
    public readonly ByteArray Copy(int start, int length)
    {
        if ((length + start) - 1 > _bytes.Length) throw new ArgumentOutOfRangeException(nameof(start));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, _bytes.Length);

        ByteArray newArray = new(length, this.Endian);
        _bytes.AsSpan(start, length).CopyTo(newArray._bytes);
        return newArray;
    }

    /// <summary>
    /// Converts the byte array into an array of characters.
    /// </summary>
    /// <returns>An array of characters.</returns>
    public readonly char[] ToCharArray()
    {
        char[] chars = new char[_bytes.Length];
        Span<byte> byteSpan = _bytes;
        Span<char> charSpan = chars;

        int subtractor = Endian == Endian.BigEndian ? 0 : byteSpan.Length - 1;

        for (int i = 0; i < byteSpan.Length; i++)
        {
            charSpan[Math.Abs(subtractor - i)] = (char)byteSpan[i];
        }

        return chars;
    }

    /// <summary>
    /// Converts the byte array in a string.
    /// </summary>
    /// <returns>A string.</returns>
    public override readonly string ToString() => new(ToCharArray());

    /// <summary>
    /// Converts the array into an unsigned short.
    /// </summary>
    /// <returns>An unsigned short.</returns>
    public readonly ushort ToUInt16() => ConvertArray(2, BitConverter.ToUInt16);

    /// <summary>
    /// Converts the array into an unsigned int.
    /// </summary>
    /// <returns>An unsigned int.</returns>
    public readonly uint ToUInt32() => ConvertArray(4, BitConverter.ToUInt32);

    /// <summary>
    /// Converts the array into an unsigned long.
    /// </summary>
    /// <returns>An unsigned long.</returns>
    public readonly ulong ToUInt64() => ConvertArray(8, BitConverter.ToUInt64);

    /// <summary>
    /// Converts the array into a signed short.
    /// </summary>
    /// <returns>A short.</returns>
    public readonly short ToInt16() => Convert.ToInt16(ToUInt16());

    /// <summary>
    /// Converts the array into a signed int.
    /// </summary>
    /// <returns>An int.</returns>
    public readonly int ToInt32() => Convert.ToInt32(ToUInt32());

    /// <summary>
    /// Converts the array into a signed long.
    /// </summary>
    /// <returns>A long.</returns>
    public readonly long ToInt64() => Convert.ToInt64(ToUInt64());

    /// <summary>
    /// Converts the array into a GUID.
    /// </summary>
    /// <returns>A GUID.</returns>
    public readonly Guid ToGuid()
    {
        if (_bytes.Length > 16)
        {
            throw new InvalidOperationException("The array is too big to be converted.");
        }
        else if (_bytes.Length < 16)
        {
            throw new InvalidOperationException("The array is too small to be a GUID.");
        }
        else
        {
            return new Guid(_bytes);
        }
    }

    /// <summary>
    /// Checks equality a given byte array against this one.
    /// </summary>
    /// <param name="obj">The byte array to check against.</param>
    /// <returns>Whether the arrays are equal.</returns>
    public override readonly bool Equals(object? obj)
    {
        if (obj == null) return false;

        ByteArray array;
        try
        {
            array = (ByteArray)obj;
        }
        catch (InvalidCastException)
        {
            return false;
        }

        return _bytes.SequenceEqual(array._bytes) && this.Endian == array.Endian;
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>A hash code.</returns>
    public override readonly int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// Checks equality of two byte arrays.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns>Whether the byte arrays are equal.</returns>
    public static bool operator ==(ByteArray a, ByteArray b)
    {
        return a.Equals(b);
    }

    /// <summary>
    /// Checks inequality of two byte arrays.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns>Whether the byte arrays are not equal.</returns>
    public static bool operator !=(ByteArray a, ByteArray b)
    {
        return !a.Equals(b);
    }

    /// <summary>
    /// Implicit conversion from byte array to ByteArray.
    /// </summary>
    /// <param name="value">The byte array</param>
    public static implicit operator ByteArray(byte[] value) => new(value);

    /// <summary>
    /// Implicit conversion from ByteArray to byte array.
    /// </summary>
    /// <param name="value">The ByteArray.</param>
    public static implicit operator byte[](ByteArray value) => value.GetBytes();
    #endregion

    #region Private Methods
    private readonly ushort ToUInt16BE()
    {
        if (_bytes.Length > 2)
        {
            throw new OverflowException("The array is too big to be converted.");
        }
        Span<byte> span = _bytes;
        return (ushort)(span[0] << 8 | span[1]);
    }

    private readonly ushort ToUInt16LE()
    {
        if (_bytes.Length > 2)
        {
            throw new OverflowException("The array is too big to be converted.");
        }
        Span<byte> span = _bytes;
        return (ushort)(span[1] << 8 | span[0]);
    }

    private readonly T ConvertArray<T>(int maxByteLength, Converter<T> converter) where T : struct
    {
        if (_bytes.Length > maxByteLength)
        {
            throw new OverflowException("The array is too big to be converted.");
        }

        ReadOnlySpan<byte> span = SystemEndian == Endian ? _bytes : [.. ((IEnumerable<byte>)_bytes).Reverse()];
        return converter(span);
    }

    #endregion
}