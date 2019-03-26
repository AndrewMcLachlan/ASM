using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Asm
{
	/// <summary>
	/// An array of bytes.
	/// </summary>
	/// <remarks>
	/// This class handles conversion of byte arrays into larger integer types using either big or little endianness.
	/// </remarks>
	[Serializable]
    [CLSCompliant(false)]
	public struct ByteArray
	{
		#region Fields
		private byte[] bytes;
		private Endian endian;
		#endregion

		#region Properties
		/// <summary>
		/// Returns a byte from the array.
		/// </summary>
		public byte this[int index]
		{
			get
			{
				return bytes[index];
			}
			set
			{
				bytes[index] = value;
			}
		}

		/// <summary>
		/// The ordinary array representaion of the bytes
		/// </summary>
		public byte[] GetBytes()
		{
            return bytes;
		}
		/// <summary>
		/// The Endianness of the array.
		/// </summary>
		public Endian Endian
		{
			get
			{
				return endian;
			}
			set
			{
				endian = value;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ByteArray"/> class.
		/// </summary>
		/// <param name="size">This number of elements in the array.</param>
		public ByteArray(int size)
		{
			bytes = new byte[size];
			endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteArray"/> class.
		/// </summary>
		/// <param name="size">This number of elements in the array.</param>
		/// <param name="type">The <see cref="Endian"/>ness of the array.</param>
		public ByteArray(int size, Endian type)
		{
			bytes = new byte[size];
			endian = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteArray"/> class.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		public ByteArray(byte[] value)
		{
			this.bytes = value;
			endian = endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteArray"/> class.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="type">The <see cref="Endian"/>ness of the array.</param>
		public ByteArray(byte[] value, Endian type)
		{
			this.bytes = value;
			endian = type;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates a copy of the array.
		/// </summary>
		/// <param name="start">The element from which to start the copy.</param>
		/// <param name="length">The number of elements to copy.</param>
		/// <returns>A new ByteArray.</returns>
		public ByteArray Copy(int start, int length)
		{
			if ((length + start)-1 > bytes.Length) throw new ArgumentOutOfRangeException("start");
            if (length > bytes.Length) throw new ArgumentOutOfRangeException("length");

			ByteArray newArray = new ByteArray(length, this.endian);
			
			int j = 0;
			for (int i=start;i<start+length;i++)
			{
				newArray.GetBytes()[j] = this.GetBytes()[i];
				j++;
			}
			return newArray;
		}

		/// <summary>
		/// Converts the byte array into an array of characters.
		/// </summary>
		/// <returns>An array of characters.</returns>
		public char[] ToCharArray()
		{
			char[] chars = new char[bytes.Length];
			int i = 0;
			foreach(byte b in this.bytes)
			{
				char c = Convert.ToChar(b);
				chars[i] = c;
				i++;
			}
			return chars;
		}

		/// <summary>
		/// Converts the byte array in a string.
		/// </summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			return new String(ToCharArray());
		}

		/// <summary>
		/// Converts the array into an unsigned short.
		/// </summary>
		/// <returns>An unsigned short.</returns>
		public ushort ToUInt16()
		{
			switch (endian)
			{
				case Endian.BigEndian :
					return ToUInt16BE();
				case Endian.LittleEndian :
					return ToUInt16LE();
				default :
					return 0;
			}
		}

		/// <summary>
		/// Converts the array into an unsigned int.
		/// </summary>
		/// <returns>An unsigned int.</returns>
		public uint ToUInt32()
		{
			switch (endian)
			{
				case Endian.BigEndian :
					return ToUInt32BE();
				case Endian.LittleEndian :
					return ToUInt32LE();
				default :
					return 0;
			}
		}

		/// <summary>
		/// Converts the array into an unsigned long.
		/// </summary>
		/// <returns>An unsigned long.</returns>
		public ulong ToUInt64()
		{
			switch (endian)
			{
				case Endian.BigEndian :
					return ToUInt64BE();
					//break;
				case Endian.LittleEndian :
					return ToUInt64LE();
					//break;
				default :
					return 0;
			}
		}

		/// <summary>
		/// Converts the array into a signed short.
		/// </summary>
		/// <returns>A short.</returns>
		public short ToInt16()
		{
			return Convert.ToInt16(ToUInt16());
		}

		/// <summary>
		/// Converts the array into a signed int.
		/// </summary>
		/// <returns>An int.</returns>
		public int ToInt32()
		{
			return Convert.ToInt32(ToUInt32());
		}

		/// <summary>
		/// Converts the array into a signed long.
		/// </summary>
		/// <returns>A long.</returns>
		public long ToInt64()
		{
			return Convert.ToInt32(ToUInt64());
		}

		/// <summary>
		/// Converts the array into a GUID.
		/// </summary>
		/// <returns>A GUID.</returns>
		public Guid ToGuid()
		{
			if (bytes.Length > 16)
			{
				throw new InvalidOperationException("The array is too big to be converted.");
			}
			else if (bytes.Length < 16)
			{
                throw new InvalidOperationException("The array is too small to be a GUID.");
			}
			else
			{
				return new Guid(bytes);
			}
		}

        /// <summary>
        /// Checks equality a given byte array against this one.
        /// </summary>
        /// <param name="obj">The byte array to check against.</param>
        /// <returns>Whether the arrays are equal.</returns>
        public override bool Equals(object obj)
        {
            ByteArray array;
            try
            {
                array = (ByteArray) obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            for (int i=0; i<bytes.Length; i++)
            {
                if (bytes[i] != array.bytes[i]) return false;
            }

            if (this.Endian != array.Endian) return false;

            return true;
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>A hashcode.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Checks equality of two byte arrays.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>Whether the byte arrays are equal.</returns>
        public static bool operator == (ByteArray a, ByteArray b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Checks inequality of two byte arrays.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>Whether the byte arrays are not equal.</returns>
        public static bool operator != (ByteArray a, ByteArray b)
        {
            return !a.Equals(b);
        }
        #endregion

		#region Private Methods
		private ushort ToUInt16BE()
		{
			if (bytes.Length > 2)
			{
				throw new OverflowException("The array is too big to be converted.");
			}
			ushort temp = 0;
			temp = bytes[0];
			temp <<= 8;
			temp |= bytes[1];

			return temp;
		}

		private ushort ToUInt16LE()
		{
			if (bytes.Length > 2)
			{
				throw new OverflowException("The array is too big to be converted.");
			}
			ushort temp = 0;
			temp = bytes[1];
			temp <<= 8;
			temp |= bytes[0];

			return temp;
		}

		private uint ToUInt32BE()
		{
			if (bytes.Length > 4)
			{
				throw new OverflowException("The array is too big to be converted.");
			}

			uint temp = 0;

			for (int i=0;i<bytes.Length;i++)
			{
				temp |= bytes[i];
				if (i+1<bytes.Length)
				{
					temp <<= 8;
				}
			}


			//temp = bytes[0];
			//temp <<= 8;
			//temp |= bytes[1];
			//temp <<= 8;
			//temp |= bytes[2];
			//temp <<= 8;
			//temp |= bytes[3];

			return temp;
		}

		private uint ToUInt32LE()
		{
			if (bytes.Length > 4)
			{
				throw new OverflowException("The array is too big to be converted.");
			}

			uint temp = 0;
			temp = bytes[3];
			temp <<= 8;
			temp |= bytes[2];
			temp <<= 8;
			temp |= bytes[1];
			temp <<= 8;
			temp |= bytes[0];

			return temp;
		}

		private ulong ToUInt64LE()
		{
			if (bytes.Length > 8)
			{
				throw new OverflowException("The array is too big to be converted.");
			}

			ulong temp = 0;
			temp = bytes[7];
			temp <<= 8;
			temp |= bytes[6];
			temp <<= 8;
			temp |= bytes[5];
			temp <<= 8;
			temp |= bytes[4];
			temp <<= 8;
			temp |= bytes[3];
			temp <<= 8;
			temp |= bytes[2];
			temp <<= 8;
			temp |= bytes[1];
			temp <<= 8;
			temp |= bytes[0];

			return temp;
		}

		private ulong ToUInt64BE()
		{
			if (bytes.Length > 8)
			{
				throw new OverflowException("The array is too big to be converted.");
			}

			ulong temp = 0;
			temp = bytes[0];
			temp <<= 8;
			temp |= bytes[1];
			temp <<= 8;
			temp |= bytes[2];
			temp <<= 8;
			temp |= bytes[3];
			temp <<= 8;
			temp |= bytes[4];
			temp <<= 8;
			temp |= bytes[5];
			temp <<= 8;
			temp |= bytes[6];
			temp <<= 8;
			temp |= bytes[7];

			return temp;
		}
		#endregion
	}



}