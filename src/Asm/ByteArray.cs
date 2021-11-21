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
		private readonly byte[] _bytes;
		private Endian _endian;
		#endregion

		#region Properties
		/// <summary>
		/// Returns a byte from the array.
		/// </summary>
		public byte this[int index]
		{
			get
			{
				return _bytes[index];
			}
			set
			{
				_bytes[index] = value;
			}
		}

		/// <summary>
		/// The ordinary array representaion of the bytes
		/// </summary>
		public byte[] GetBytes()
		{
            return _bytes;
		}
		/// <summary>
		/// The Endianness of the array.
		/// </summary>
		public Endian Endian
		{
			get
			{
				return _endian;
			}
			set
			{
				_endian = value;
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
			_bytes = new byte[size];
			_endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteArray"/> class.
		/// </summary>
		/// <param name="size">This number of elements in the array.</param>
		/// <param name="type">The <see cref="Endian"/>ness of the array.</param>
		public ByteArray(int size, Endian type)
		{
			_bytes = new byte[size];
			_endian = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteArray"/> class.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		public ByteArray(byte[] value)
		{
			this._bytes = value;
			_endian = _endian = BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteArray"/> class.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="type">The <see cref="Endian"/>ness of the array.</param>
		public ByteArray(byte[] value, Endian type)
		{
			this._bytes = value;
			_endian = type;
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
			if ((length + start)-1 > _bytes.Length) throw new ArgumentOutOfRangeException(nameof(start));
            if (length > _bytes.Length) throw new ArgumentOutOfRangeException(nameof(length));

			ByteArray newArray = new(length, this._endian);

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
			char[] chars = new char[_bytes.Length];
			int i = 0;
			foreach(byte b in this._bytes)
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
            return _endian switch
            {
                Endian.BigEndian => ToUInt16BE(),
                Endian.LittleEndian => ToUInt16LE(),
                _ => 0,
            };
        }

		/// <summary>
		/// Converts the array into an unsigned int.
		/// </summary>
		/// <returns>An unsigned int.</returns>
		public uint ToUInt32()
		{
            return _endian switch
            {
                Endian.BigEndian => ToUInt32BE(),
                Endian.LittleEndian => ToUInt32LE(),
                _ => 0,
            };
        }

		/// <summary>
		/// Converts the array into an unsigned long.
		/// </summary>
		/// <returns>An unsigned long.</returns>
		public ulong ToUInt64()
		{
            return _endian switch
            {
                Endian.BigEndian => ToUInt64BE(),
                //break;
                Endian.LittleEndian => ToUInt64LE(),
                //break;
                _ => 0,
            };
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
        public override bool Equals(object? obj)
        {
			if (obj == null) return false;

            ByteArray array;
            try
            {
                array = (ByteArray) obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            for (int i=0; i<_bytes.Length; i++)
            {
                if (_bytes[i] != array._bytes[i]) return false;
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
			if (_bytes.Length > 2)
			{
				throw new OverflowException("The array is too big to be converted.");
			}
            ushort temp = _bytes[0];
            temp <<= 8;
			temp |= _bytes[1];

			return temp;
		}

		private ushort ToUInt16LE()
		{
			if (_bytes.Length > 2)
			{
				throw new OverflowException("The array is too big to be converted.");
			}
            ushort temp = _bytes[1];
            temp <<= 8;
			temp |= _bytes[0];

			return temp;
		}

		private uint ToUInt32BE()
		{
			if (_bytes.Length > 4)
			{
				throw new OverflowException("The array is too big to be converted.");
			}

			uint temp = 0;

			for (int i=0;i<_bytes.Length;i++)
			{
				temp |= _bytes[i];
				if (i+1<_bytes.Length)
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
			if (_bytes.Length > 4)
			{
				throw new OverflowException("The array is too big to be converted.");
			}
            uint temp = _bytes[3];
            temp <<= 8;
			temp |= _bytes[2];
			temp <<= 8;
			temp |= _bytes[1];
			temp <<= 8;
			temp |= _bytes[0];

			return temp;
		}

		private ulong ToUInt64LE()
		{
			if (_bytes.Length > 8)
			{
				throw new OverflowException("The array is too big to be converted.");
			}
            ulong temp = _bytes[7];
            temp <<= 8;
			temp |= _bytes[6];
			temp <<= 8;
			temp |= _bytes[5];
			temp <<= 8;
			temp |= _bytes[4];
			temp <<= 8;
			temp |= _bytes[3];
			temp <<= 8;
			temp |= _bytes[2];
			temp <<= 8;
			temp |= _bytes[1];
			temp <<= 8;
			temp |= _bytes[0];

			return temp;
		}

		private ulong ToUInt64BE()
		{
			if (_bytes.Length > 8)
			{
				throw new OverflowException("The array is too big to be converted.");
			}
            ulong temp = _bytes[0];
            temp <<= 8;
			temp |= _bytes[1];
			temp <<= 8;
			temp |= _bytes[2];
			temp <<= 8;
			temp |= _bytes[3];
			temp <<= 8;
			temp |= _bytes[4];
			temp <<= 8;
			temp |= _bytes[5];
			temp <<= 8;
			temp |= _bytes[6];
			temp <<= 8;
			temp |= _bytes[7];

			return temp;
		}
		#endregion
	}



}