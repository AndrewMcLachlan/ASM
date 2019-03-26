using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Asm
{
    /// <summary>
    /// An array of bits in either little endian or big endian format.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), Serializable]
    [CLSCompliant(false)]
    public sealed class ExtendedBitArray : ICollection, IEnumerable, ICloneable
    {
        #region Fields
        private byte[] bytes;
        //private bool[] bitArray;
        private BitArray bitArray;
        private Endian endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the value of the bit at the given position.
        /// </summary>
        public bool this[int index]
        {
            get
            {
                return bitArray[index];
            }
        }

        /// <summary>
        /// Gets the endianness of the array.
        /// </summary>
        public Endian Endian
        {
            get
            {
                return endian;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="values">The bitarray to extend.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(BitArray values, Endian endian)
        {
            this.endian = endian;
            bitArray = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="values">An array of boolean values representing individual bits.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(bool[] values, Endian endian)
        {
            this.endian = endian;
            bitArray = GetBits(values, endian);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="value">A byte array to convert.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(byte[] value, Endian endian)
        {
            this.endian = endian;
            this.bytes = Reverse(value);
            GetBits();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="value">A byte value to convert.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(byte value, Endian endian)
        {
            this.bytes = new byte[] { value };
            this.endian = endian;
            GetBits();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="value">A signed-byte value to convert.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(sbyte value, Endian endian)
        {
            this.bytes = new byte[] { Convert.ToByte(value) };
            this.endian = endian;
            GetBits();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="value">A 16 bit integer value to convert.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(short value, Endian endian)
        {
            this.bytes = BitConverter.GetBytes(value);
            this.endian = endian;
            GetBits();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="value">An integer value to convert.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(int value, Endian endian)
        {
            this.bytes = BitConverter.GetBytes(value);
            this.endian = endian;
            GetBits();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBitArray"/> class.
        /// </summary>
        /// <param name="value">A 64 bit integer value to convert.</param>
        /// <param name="endian">The endianess of the array.</param>
        public ExtendedBitArray(long value, Endian endian)
        {
            this.bytes = BitConverter.GetBytes(value);
            this.endian = endian;
            GetBits();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the byte representation of the array.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] GetBytes()
        {
            return bytes;
        }

        /// <summary>
        /// Creates a copy of part of the array.
        /// </summary>
        /// <param name="start">Where the new array will start from.</param>
        /// <param name="length">The length of the new array.</param>
        /// <returns>A new ExtendedBitArray.</returns>
        public ExtendedBitArray Copy(int start, int length)
        {
            if ((length + start)-1 > this.Count) throw new ArgumentOutOfRangeException("start");
            if (length > this.Count) throw new ArgumentOutOfRangeException("length");

            bool[] temp = new bool[length-start];

            for (int i=start, j=0;i<length;i++, j++)
            {
                temp[j] = this[i];
            }

            return new ExtendedBitArray(temp, this.Endian);
        }

        /// <summary>
        /// Creates a copy of part of the array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="index">The index whehre copying begins.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="index"/> is less than 0 or greater than the length of <paramref name="array"/>.</exception>
        public void CopyTo(ExtendedBitArray array, int index)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (index < 0) throw new ArgumentException("index cannot be less than 0.");
            if (index > array.bitArray.Length - 1) throw new ArgumentException("index cannot be greater than the length of the array.");

            bool[] temp = new bool[array.bitArray.Length];
            array.bitArray.CopyTo(temp, 0);
            bitArray.CopyTo(temp, index);
            array.bitArray = new BitArray(temp);
        }

        /// <summary>
        /// Returns the signed 8 bit integer representation of the array.
        /// </summary>
        /// <returns>An signed byte.</returns>
        /// <remarks>
        /// If the array is greater than 8 bits long, only the first 8 bits will be used.
        /// </remarks>
        public sbyte ToSByte()
        {
            return Convert.ToSByte(ToSigned(1));
        }

        /// <summary>
        /// Returns the signed 16 bit integer representation of the array.
        /// </summary>
        /// <returns>An signed short.</returns>
        /// <remarks>
        /// If the array is greater than 16 bits long, only the first 16 bits will be used.
        /// </remarks>
        public short ToInt16()
        {
            return Convert.ToInt16(ToSigned(2));
        }

        /// <summary>
        /// Returns the signed 32 bit integer representation of the array.
        /// </summary>
        /// <returns>An signed byte.</returns>
        /// <remarks>
        /// If the array is greater than 32 bits long, only the first 32 bits will be used.
        /// </remarks>
        public int ToInt32()
        {
            return Convert.ToInt32(ToSigned(4));
        }

        /// <summary>
        /// Returns the signed 64 bit integer representation of the array.
        /// </summary>
        /// <returns>An signed byte.</returns>
        /// <remarks>
        /// If the array is greater than 64 bits long, only the first 64 bits will be used.
        /// </remarks>
        public long ToInt64()
        {
            return Convert.ToInt64(ToSigned(8));
        }

        /// <summary>
        /// Returns the unsigned 8 bit integer representation of the array.
        /// </summary>
        /// <returns>An unsigned byte.</returns>
        /// <remarks>
        /// If the array is greater than 8 bits long, only the first 8 bits will be used.
        /// </remarks>
        public byte ToByte()
        {
            return Convert.ToByte(ToUnsigned(1));
        }

        /// <summary>
        /// Returns the unsigned 16 bit integer representation of the array.
        /// </summary>
        /// <returns>An unsigned 16 bit integer.</returns>
        /// <remarks>
        /// If the array is greater than 16 bits long, only the first 16 bits will be used.
        /// </remarks>
        public ushort ToUInt16()
        {
            return Convert.ToUInt16(ToUnsigned(2));
        }

        /// <summary>
        /// Returns the unsigned 32 bit integer representation of the array.
        /// </summary>
        /// <returns>An unsigned 32 bit integer.</returns>
        /// <remarks>
        /// If the array is greater than 32 bits long, only the first 32 bits will be used.
        /// </remarks>
        public uint ToUInt32()
        {
            return Convert.ToUInt32(ToUnsigned(4));
        }

        /// <summary>
        /// Returns the unsigned 64 bit integer representation of the array.
        /// </summary>
        /// <returns>An unsigned 64 bit integer.</returns>
        /// <remarks>
        /// If the array is greater than 64 bits long, only the first 64 bits will be used.
        /// </remarks>
        public ulong ToUInt64()
        {
            return Convert.ToUInt64(ToUnsigned(8));
        }

        /// <summary>
        /// Returns the string representation of the bits in the array.
        /// </summary>
        /// <returns>A string of binary digits.</returns>
        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();

            foreach (bool b in this)
            {
                temp.Append(Convert.ToInt16(b).ToString());
            }

            return temp.ToString();
        }

        #region /**/
        /*public static byte[] ToLittleEndian(byte[] bytes)
		{
			ExtendedBitArray b;
			byte[] output = new byte[bytes.Length];
			for(int i=0, j=bytes.Length-1;i<bytes.Length;i++, j--)
			{
				b = new ExtendedBitArray(bytes[i], Endian.BigEndian);
			}

			return output;
		}*/
        #endregion
        #endregion

        #region Private Methods
        private static void GetBits()
        {
            /*bitArray = new bool[this.bytes.Length * 8];
            short quotient;
            byte element;

            for (int i=0;i<this.bytes.Length;i++)
            {
                element = bytes[i];
                quotient = bytes[i];

                for (int j=0;j<8 && quotient > 0;j++)
                {
                    bitArray[j+(i*8)] = Convert.ToBoolean(element % 2);
                    element = Convert.ToByte(element / 2);
                }
            }

            switch (this.endian)
            {
                case Endian.BigEndian :
                    for (int i=bitArray.Length-1, k=0;i>=0;i--,k++)
                    {
                        thisCollection[k] = bitArray[i];
                    }
                    break;
                case Endian.LittleEndian :
                    //foreach (bool b in bitArray)
                    for (int i=0;i<bitArray.Length;i++)
                    {
                        thisCollection[i] = bitArray[i];
                    }
                    break;
            }*/
        }

        private static BitArray GetBits(bool[] values, Endian endian)
        {
            BitArray temp = new BitArray(values.Length);

            switch ((endian == Endian.LittleEndian) && BitConverter.IsLittleEndian)
            {
                case false:
                    for (int i=values.Length-1, k=0;i>=0;i--, k++)
                    {
                        temp[k] = values[i];
                    }
                    break;
                case true:
                    for (int i=0;i<values.Length;i++)
                    {
                        temp[i] = values[i];
                    }
                    break;
            }

            return temp;
        }

        private static byte[] Reverse(byte[] bytes)
        {
            byte[] newBytes = new byte[bytes.Length];

            for (int i=0, j=bytes.Length-1;i<bytes.Length;i++, j--)
            {
                newBytes[j] = bytes[i];
            }

            return newBytes;
        }

        private long ToSigned(int BytesToRead)
        {
            long temp = 0;
            switch (this.endian)
            {
                case Endian.BigEndian:
                    switch ((bool)this[0])
                    {
                        //Negative
                        case true:
                            for (int i=0, j=this.Count-2;i<this.Count-1 && i<(BytesToRead*8)-1;i++, j--)
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
                            for (int i=0, j=this.Count-2;i<this.Count-1 && i<(BytesToRead*8)-1;i++, j--)
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
                    switch ((bool)this[this.Count-1])
                    {
                        case true:
                            for (int i=0;i<this.Count-1;i++)
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
                            for (int i=0;i<this.Count-1;i++)
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

        private ulong ToUnsigned(int BytesToRead)
        {
            ulong temp = 0;
            switch (this.endian)
            {
                case Endian.BigEndian:
                    for (int i=0, j=this.Count-1;i<this.Count && i<BytesToRead*8;i++, j--)
                    {
                        if (this[i])
                        {
                            temp += (ulong)Math.Pow(2, j);
                        }
                    }
                    break;
                case Endian.LittleEndian:
                    for (int i=0;i<this.Count;i++)
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
            bitArray.CopyTo(array, index);
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
            get { return bitArray.Count; }
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
            get { return bitArray.IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ExtendedBitArray"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="ExtendedBitArray"/>.
        /// </returns>
        public object SyncRoot
        {
            get { return bitArray.SyncRoot; }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ExtendedBitArray"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator"/> for the entire <see cref="ExtendedBitArray"/>.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return bitArray.GetEnumerator();
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clones this <see cref="ExtendedBitArray"/>.
        /// </summary>
        /// <returns>A copy fo this <see cref="ExtendedBitArray"/>.</returns>
        public object Clone()
        {
            return new ExtendedBitArray((BitArray) bitArray.Clone(), this.endian);
        }
        #endregion
    }
}
