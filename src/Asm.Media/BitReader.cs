using System;

namespace AscentMedia.Utils
{
	/// <summary>
	/// Summary description for BitReader.
	/// </summary>
	public static class BitReader
	{
        /// <summary>
        /// Retrieves a single bit from a byte.
        /// </summary>
        /// <param name="value">The byte.</param>
        /// <param name="position">The position of the bit to retrieve.</param>
        /// <returns>A boolean value representing the bit.</returns>
		public static bool GetBitAtPosition(byte value, int position)
		{
			bool[] bitArray = ToBitBoolean(value);
			return bitArray[position];
		}

        /// <summary>
        /// Converts a byte to an array of bits.
        /// </summary>
        /// <param name="value">The bye to convert.</param>
        /// <returns>An array of boolean vlaues representing the bits.</returns>
		public static bool[] ToBitBoolean(byte value)
		{
			// Get the value of the byte.
			int myNum = System.Convert.ToInt32(value);

			// Make a bool array for the bits.
			// 0 == false
			// 1 == true
			bool[] myByte = new bool[8];
			for (int i = 0; i < 8; i++)
			{
				// ANDing against each bit to set the bool value
				if ((myNum & (1 << (7 - i))) != 0)	
				{
					myByte[i] = true;
				}
			}
			return myByte;
		}

        /// <summary>
        /// Converts a byte to an array of bits.
        /// </summary>
        /// <param name="value">The byte to convert.</param>
        /// <returns>An array of char vlaues representing the bits.</returns>
		public static char[] ToBitChar(byte value)
		{
			char[] myChar = new char[8];

			bool[] myBool = ToBitBoolean(value);
			for (int i = 0; i < 8; i++) 
			{
				if (myBool[i])
				{
					myChar[i] = System.Convert.ToChar("1");
				}
				else
				{
					myChar[i] = System.Convert.ToChar("0");
				}
			}
			return myChar;	
			
		}

        /// <summary>
        /// Converts a byte array to an array of bits.
        /// </summary>
        /// <param name="value">The byte array to convert.</param>
        /// <returns>An array of char vlaues representing the bits.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
		public static char[] ToBitChars(byte[] value)
		{
            if (value == null) throw new ArgumentNullException("value");

			char[] myChars = new char[value.Length * 8];
			int i = 0;
			foreach (byte b in value)
			{
				bool[] bit = ToBitBoolean( b );
				for (int j = 0; j < 8; j++)
				{
					if (bit[j])
					{
						myChars[i++] = System.Convert.ToChar("1");
					}
					else
					{
						myChars[i++] = System.Convert.ToChar("0");
					}
				}
			}
			return myChars;
		}

        /// <summary>
        /// Converts a byte array to an array of bits.
        /// </summary>
        /// <param name="value">The byte array to convert.</param>
        /// <returns>An array of boolean vlaues representing the bits.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
		public static bool[] ToBitBooleans(byte[] value)
		{
            if (value == null) throw new ArgumentNullException("value");

			bool[] myBools = new bool[value.Length * 8 ];
			int i = 0;
			foreach (byte b in value)
			{
				bool[] bit = ToBitBoolean( b );
				for (int j = 0; j < 8; j++) 
				{
					if (bit[j])
					{
						myBools[i++] = true;
					}
					else
					{
						myBools[i++] = false;
					}
				}
			}
			return myBools;
		}

        /// <summary>
        /// Converts an array of chars into a byte.
        /// </summary>
        /// <param name="value">The char array to convert.</param>
        /// <returns>The first 8 characters in the array as a byte.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the length of <paramref name="value"/> is less than 8.</exception>
		public static byte ToByteChar(char[] value)
		{
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length < 8) throw new ArgumentException("value must have a length greater than or equal to 8", "value");

			int myInt = 0;
			for (int i = 0; i < 8 ; i++ )
			{
				if ( value[i].Equals("1"))
					myInt += (int)Math.Pow(2, 7 - i);
			}
			return System.Convert.ToByte(myInt);
		}

        /// <summary>
        /// Converts an array of boolean values into a byte.
        /// </summary>
        /// <param name="value">The char array to convert.</param>
        /// <returns>The first 8 values in the array as a byte.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the length of <paramref name="value"/> is less than 8.</exception>
		public static byte ToByteBoolean(bool[] value)
		{
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length < 8) throw new ArgumentException("value must have a length greater than or equal to 8", "value");

			int myInt = 0;
			for (int i = 0; i < 8 ; i++ )
			{
				if ( value[i])
					myInt += (int)Math.Pow(2, 7 - i);
			}
			return System.Convert.ToByte(myInt);
		}
	}
}
