using AscentMedia.Utils;
using System;
using System.Collections;
using System.IO;

namespace Asm.Media
{
	/// <summary>
	/// Represents an MPEG movie file.
	/// </summary>
	public class Mpeg : MediaFile
	{
		#region Fields
		//private long audioBytes;
		//private long fileSize;

		private string version;
		private string layer;
		private bool protection;
		private string frequency;
        private bool padding;
        private bool privateMarker;
        private string channelMode;
        private string modeExtension;
		private bool copyRight;
		private bool original;
        private string emphasis;
		private long headerPosition;
		#endregion

		#region Properties
		/// <summary>
		/// The duration of the movie in seconds.
		/// </summary>
		public override double Duration
		{
			get
			{
				return ((this.Size*8) / this.Bitrate) * MPEGsystemNonOverheadPercentage((int) this.VideoBitrate, (int) this.AudioBitrate);
			}
		}

		/// <summary>
		/// The total bitrate (audio and video streams) of the file in b/s.
		/// </summary>
		/// <remarks>
        /// To get the bitrate in Kb/s, divide this value by the <see cref="MediaFile.KilobitConverter"/> constant.
		/// </remarks>
		public override double Bitrate
		{
			get
			{
				return base.Bitrate;
			}
		}

        /// <summary>
        /// Frequency.
        /// </summary>
        public string Frequency
        {
            get { return frequency; }
        }

        /// <summary>
        /// Emphasis
        /// </summary>
        public string Emphasis
        {
            get { return emphasis; }
        }

        /// <summary>
        /// Copyright.
        /// </summary>
        public bool Copyright
        {
            get { return copyRight; }
        }

        /// <summary>
        /// Channel Mode.
        /// </summary>
        public string ChannelMode
        {
            get { return channelMode; }
        }

        /// <summary>
        /// Mode Extension.
        /// </summary>
        public string ModeExtension
        {
            get { return modeExtension; }
        }

        /// <summary>
        /// Padding.
        /// </summary>
        public bool Padding
        {
            get { return padding; }
        }

        /// <summary>
        /// Private.
        /// </summary>
        public bool Private
        {
            get { return privateMarker; }
            set { privateMarker = value; }
        }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of the <see cref="Mpeg"/> class.
		/// </summary>
		/// <param name="fileName">The name and path of the file to read.</param>
		public Mpeg(string fileName) : base(fileName)
		{
			this.MediaType = "MPEG";
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Retrieve information from a movie header.
		/// </summary>
		/// <param name="parent">The parent header to which the following header belongs. Null values accepted.</param>
		/// <param name="data">The binary reader attached to the file.</param>
		/// <param name="start">The position in the file (in bytes) to start reading from.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <param name="headerOffset">Any offset relating to the previous header.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
		protected override void GetMetadata(Header parent, BinaryReader data, long start, long length, short headerOffset)
		{
            if (data == null) throw new ArgumentNullException("data");

			byte[] headerBytes = FindAudioHeader(data);
			if (headerBytes != null)
			{
				ParseAudioHeader(headerBytes);
			}

			data.BaseStream.Position = 0;

			//Find the movie sequence header
			while (data.BaseStream.Position < data.BaseStream.Length)
			{
				ByteArray b = new ByteArray(4, Endian.BigEndian);
                data.Read(b.GetBytes(), 0, 4);

				//435 is 00 00 01 B3 in hex notation. This indicates the video header.
				if (b.ToUInt32() == 435)
				{
					//ByteArray height = new ByteArray(3);
					//ByteArray width = new ByteArray(3);

					byte[] heightwidth = new byte[3];
					data.Read(heightwidth, 0, 3);

					ExtendedBitArray mbits = new ExtendedBitArray(heightwidth, Endian.BigEndian);

					uint testHeight = mbits.Copy(0,12).ToUInt32();
					uint testWidth = mbits.Copy(12,12).ToUInt32();

					this.Height = (int) testHeight;
					this.Width = (int) testWidth;

					//Get the bitrate
					//Move along past aspect ratio and frame rate
					data.BaseStream.Position += 1;

					ByteArray bitrate = new ByteArray(3, Endian.BigEndian);
                    data.Read(bitrate.GetBytes(), 0, 3);

                    ExtendedBitArray mbitrate = new ExtendedBitArray(bitrate.GetBytes(), Endian.BigEndian);
					uint iBitrate = mbitrate.Copy(0,18).ToUInt32();

					this.VideoBitrate = (iBitrate*400);
					this.Bitrate = (iBitrate*400) + this.Bitrate;

					byte[] assortedInfo = new byte[1];
					data.Read(assortedInfo, 0, 1);
					ExtendedBitArray mAssortedInfo = new ExtendedBitArray(assortedInfo, Endian.BigEndian);
					bool load_intra_quantizer_matrix = Convert.ToBoolean(mAssortedInfo.Copy(6,1).ToUInt32());
					bool load_non_intra_quantizer_matrix = false;
					if (load_intra_quantizer_matrix)
					{
						data.BaseStream.Position += 63;
						byte[] lastByte = new byte[1];
						data.Read(lastByte, 0, 1);
						ExtendedBitArray load_non_intra_quantizer_matrixCheck = new ExtendedBitArray(lastByte, Endian.BigEndian);
						load_non_intra_quantizer_matrix = Convert.ToBoolean(load_non_intra_quantizer_matrixCheck.Copy(7,1).ToUInt32());
						if (load_non_intra_quantizer_matrix)
						{
							data.BaseStream.Position += 64;
						}
					}
					else
					{
						load_non_intra_quantizer_matrix	= Convert.ToBoolean(mAssortedInfo.Copy(7,1).ToUInt32());
					}

					//Check for the MPEG-2 extension header 00 00 01 B5 (437)
					ByteArray b2 = new ByteArray(4, Endian.BigEndian);
                    data.Read(b2.GetBytes(), 0, 4);
					if (b2.ToUInt32() == 437)
					{
						this.MediaType = "MPEG2";
					}
					else
					{
						this.MediaType = "MPEG1";
					}

					//Read the extension header if was found
					if (this.MediaType == "MPEG2")
					{
						byte[] extension_id_byte = new byte[1];
						data.Read(extension_id_byte, 0, 1);
						ExtendedBitArray extension_id_bits = new ExtendedBitArray(extension_id_byte, Endian.BigEndian);
						uint extension_id = extension_id_bits.Copy(0,4).ToUInt32();

						//Sequence Extension
						if (extension_id == 1)
						{
							byte[] sequence_extension_bytes = new byte[4];
							data.Read(sequence_extension_bytes, 0, 4);
							ExtendedBitArray sequence_extension_bits = new ExtendedBitArray(sequence_extension_bytes, Endian.BigEndian);
							uint bit_rate_extension = sequence_extension_bits.Copy(11,12).ToUInt32();
							System.Diagnostics.Debug.WriteLine(bit_rate_extension.ToString());
						}
					}
					break;
				}

				//Set the stream backwards to check the next byte sequence
				data.BaseStream.Position -= 3;
			}
		}
        #endregion

        #region Private Methods
        /*
		private void CalculateLength()
		{
			System.IO.FileInfo fi = new System.IO.FileInfo(this.Filename);
			this.fileSize = fi.Length;
			this.audioBytes = this.fileSize - this.headerPosition;
			int bitrate = System.Convert.ToInt32(this.Bitrate);
			if (bitrate > 0)
			{
				this.Duration = (this.audioBytes * 8) / (1000 * Bitrate);
			}
		}
        */
		private void ParseAudioHeader(byte[] headerBytes)
		{
			ExtendedBitArray boolHeader = new ExtendedBitArray(headerBytes, Endian.BigEndian);
			ParseVersion(boolHeader[11], boolHeader[12]);
			ParseLayer(boolHeader[13], boolHeader[14]);
			this.protection = boolHeader[15];
			Parsebitrate(boolHeader[16], boolHeader[17], boolHeader[18], boolHeader[19]);
			ParseFrequency(boolHeader[20], boolHeader[21]);
			this.padding = boolHeader[22];
			this.Private = boolHeader[23];
			ParseChannelMode(boolHeader[24], boolHeader[25]);
			ParseModeExtension(boolHeader[26], boolHeader[27]);
			this.copyRight = boolHeader[28];
			this.original = boolHeader[29];
			ParseEmphasis(boolHeader[30], boolHeader[31]);

		}

		private void ParseFrequency ( bool b1, bool b2)
		{
			if (b1)
			{
				if(b2)
				{
					//"11"
					this.frequency = "reserved";
				}
				else
				{
					// "01"
					switch (this.version)
					{
						case "MPEG Version 1":
							this.frequency = "32000";
							break;
						case "MPEG Version 2":
							this.frequency = "16000";
							break;
						case "MPEG Version 2.5":
							this.frequency = "8000";
							break;
					}
				}
			}
			else
			{
				if (b2)
				{
					//"01"
					switch (this.version)
					{
						case "MPEG Version 1":
							this.frequency = "32000";
							break;
						case "MPEG Version 2":
							this.frequency = "16000";
							break;
						case "MPEG Version 2.5":
							this.frequency = "8000";
							break;
					}
				}
				else
				{
					// "00"
					switch (this.version)
					{
						case "MPEG Version 1":
							this.frequency = "44100";
							break;
						case "MPEG Version 2":
							this.frequency = "22050";
							break;
						case "MPEG Version 2.5":
							this.frequency = "11025";
							break;
					}
				}
			}

		}

		private void ParseModeExtension (bool b1, bool b2)
		{

			if (b1)
			{
				if (b2)
				{
					if (this.layer.Equals("Layer III"))
					{
						// "11", L3
						this.modeExtension = "IS+MS";
					}
					else
					{
						// "11", L1 or L2
						this.modeExtension = "16-31";
					}
				}
				else
				{
					if (this.layer.Equals("Layer III"))
					{
						// "10", L3
						this.modeExtension = "MS";
					}
					else
					{
						// "10", L1 or L2
						this.modeExtension = "12-31";
					}
				}
			}
			else
			{
				if (b2)
				{
					if (this.layer.Equals("Layer III"))
					{
						// "01", L3
						this.modeExtension = "IS";
					}
					else
					{
						// "01", L1 or L2
						this.modeExtension = "8-31";
					}
				}
				else
				{
					if (this.layer.Equals("Layer III"))
					{
						// "00", L3
						this.modeExtension = "";
					}
					else
					{
						// "00", L1 or L2
						this.modeExtension = "4-31";
					}
				}
			}
		}

		private void ParseEmphasis (bool b1, bool b2)
		{
			//00 - none
			//01 - 50/15 ms
			//10 - reserved
			//11 - CCIT J.17

			if (b1)
			{
				if(b2)
				{
					//"11"
					this.emphasis = "CCIT J.17";
				}
				else
				{
					//"10"
					this.emphasis = "reserved";
				}
			}
			else
			{
				if (b2)
				{
					//"01"
					this.emphasis = "50/15 ms";
				}
				else
				{
					//"00"
					this.emphasis = "none";
				}
			}

		}

		private void ParseChannelMode (bool b1, bool b2)
		{
			//00 - Stereo
			//01 - Joint stereo (Stereo)
			//10 - Dual channel (Stereo)
			//11 - Single channel (Mono)
			if (b1)
			{
				if(b2)
				{
					//"11"
					this.channelMode = "Single Channel";
				}
				else
				{
					//"10"
					this.channelMode = "Dual Channel";
				}
			}
			else
			{
				if (b2)
				{
					//"01"
					this.channelMode = "Joint Stereo";
				}
				else
				{
					//"00"
					this.channelMode = "Stereo";
				}
			}

		}

		private void ParseVersion (bool b1, bool b2)
		{
			// get the MPEG Audio Version ID
			//MPEG Audio version ID
			//00 - MPEG Version 2.5
			//01 - reserved
			//10 - MPEG Version 2 (ISO/IEC 13818-3)
			//11 - MPEG Version 1 (ISO/IEC 11172-3)
			if (b1)
			{
				if(b2)
				{
					this.version = "MPEG Version 1";
				}
				else
				{
					this.version = "MPEG Version 2";
				}
			}
			else
			{
				if (b2)
				{
					this.version = "reserved";
				}
				else
				{
					this.version = "MPEG Version 2.5";
				}
			}

		}

		private void ParseLayer (bool b1, bool b2)
		{
			if (b1)
			{
				if(b2)
				{
					// if "11"
					this.layer = "Layer I";
				}
				else
				{
					// "10"
					this.layer = "Layer II";
				}
			}
			else
			{
				if (b2)
				{
					// "01"
					this.layer = "Layer III";
				}
				else
				{
					// "00"
					this.layer = "reserved";
				}
			}
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void Parsebitrate (bool b1, bool b2, bool b3, bool b4)
		{
			//string str = "b" + Convert.ToInt32(b1).ToString() + Convert.ToInt32(b2).ToString() + Convert.ToInt32(b3).ToString() + Convert.ToInt32(b4).ToString() + "_" + this.Version.Substring(this.Version.Length-1) + this.Layer.Substring(this.Layer.LastIndexOf(" ")+1);

			// I know there is a more elegant way than this.
			#region 1xxx
			if (b1)
			{
				if (b2)
				{
					if (b3)
					{
						if (b4)
						{
							#region "1111"
							//0 == bad
							this.Bitrate = 0;
							#endregion
						}
						else
						{
							#region "1110"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 448;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 384;
								}
								else
								{
									// V1, L3
									this.Bitrate = 320;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 256;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 160;
								}
							}
							#endregion
						}
					}
					else
					{
						if (b4)
						{
							#region "1101"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 416;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 320;
								}
								else
								{
									// V1, L3
									this.Bitrate = 256;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 224;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 144;
								}
							}
							#endregion
						}
						else
						{
							#region "1100"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 384;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 256;
								}
								else
								{
									// V1, L3
									this.Bitrate = 224;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 192;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 128;
								}
							}
							#endregion
						}
					}
				}
				else //b2 not set
				{
					if (b3)
					{
						if (b4)
						{
							#region "1011"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 352;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 224;
								}
								else
								{
									// V1, L3
									this.Bitrate = 192;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 176;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 112;
								}
							}
							#endregion
						}
						else
						{
							#region "1010"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 320;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 192;
								}
								else
								{
									// V1, L3
									this.Bitrate = 160;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 160;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 96;
								}
							}
							#endregion
						}
					}
					else
					{
						if (b4)
						{
							#region "1001"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 288;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 160;
								}
								else
								{
									// V1, L3
									this.Bitrate = 128;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 144;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 80;
								}
							}
							#endregion
						}
						else
						{
							#region "1000"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 256;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 128;
								}
								else
								{
									// V1, L3
									this.Bitrate = 112;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 128;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 64;
								}
							}
							#endregion
						}
					}
				}
			}
			#endregion
			#region 0xxx
			else
			{
				if (b2)
				{
					if (b3)
					{
						if (b4)
						{
							#region "0111"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 224;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 112;
								}
								else
								{
									// V1, L3
									this.Bitrate = 96;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 112;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 56;
								}
							}
							#endregion
						}
						else
						{
							#region "0110"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 192;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 96;
								}
								else
								{
									// V1, L3
									this.Bitrate = 80;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 96;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 48;
								}
							}
							#endregion
						}
					}
					else
					{
						if (b4)
						{
							#region "0101"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 160;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 80;
								}
								else
								{
									// V1, L3
									this.Bitrate = 64;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 80;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 40;
								}
							}
							#endregion
						}
						else
						{
							#region "0100"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 128;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 64;
								}
								else
								{
									// V1, L3
									this.Bitrate = 56;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 64;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 32;
								}
							}
							#endregion
						}
					}
				}
				else //b2 not set
				{
					if (b3)
					{
						if (b4)
						{
							#region "0011"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 96;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 56;
								}
								else
								{
									// V1, L3
									this.Bitrate = 48;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 56;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 24;
								}
							}
							#endregion
						}
						else
						{
							#region "0010"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 64;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 48;
								}
								else
								{
									// V1, L3
									this.Bitrate = 40;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 48;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 16;
								}
							}
							#endregion
						}
					}
					else
					{
						if (b4)
						{
							#region "0001"
							if (this.version.EndsWith("1"))
							{
								if (this.layer.EndsWith(" I"))
								{
									// V1, L1
									this.Bitrate = 32;
								}
								else if (this.layer.EndsWith(" II"))
								{
									// v1, L2
									this.Bitrate = 32;
								}
								else
								{
									// V1, L3
									this.Bitrate = 32;
								}
							}
							else
							{
								if (this.layer.EndsWith(" I"))
								{
									// V2, L1
									this.Bitrate = 32;
								}
								else
								{
									// V2, L2 & L3
									this.Bitrate = 8;
								}
							}
							#endregion
						}
						else
						{
							#region "0000"
							//1 = free
							this.Bitrate = 1;
							#endregion
						}
					}
				}
			}

			#endregion
			this.Bitrate = this.Bitrate * 1000;
			//this.Bitrate = ((int) list[str]) * 1000.0;
			this.AudioBitrate = this.Bitrate;
		}

		private byte[] FindAudioHeader(BinaryReader Data)
		{
			long maxlength = (Data.BaseStream.Length > 1024*1024) ? 1024*1024 : Data.BaseStream.Length;
			while (Data.BaseStream.Position < maxlength)
			{
				ByteArray b = new ByteArray(4, Endian.BigEndian);
                Data.Read(b.GetBytes(), 0, 4);

				//448 is 00 00 01 C0 in hex notation. This marks the beginning of the audio header.
				if (b.ToUInt32() == 448)
				{

					Data.BaseStream.Position += 8;
					//Find the first FF.
					while (Data.BaseStream.ReadByte() != 255);
					Data.BaseStream.Position--;

					this.headerPosition = Data.BaseStream.Position;
					byte[] retByte = new byte [4];
					retByte[0] = Data.ReadByte();
					retByte[1] = Data.ReadByte();
					retByte[2] = Data.ReadByte();
					retByte[3] = Data.ReadByte();
					return retByte;
				}

				Data.BaseStream.Position -= 3;
			}
			return null;
		}

		/// <summary>
		/// Calculates the overhead of the file as a percentage multiplier
		/// </summary>
		/// <param name="VideoBitrate">The video bitrate in (b/s).</param>
		/// <param name="AudioBitrate">The audio bitrate (in b/s).</param>
		/// <returns>A percentage multiplier (&lt; 1).</returns>
		/// <remarks>
		/// Interleaved MPEG audio/video files have a certain amount of overhead that varies
		/// by both video and audio bitrates, and not in any sensible, linear/logarithmic pattern.
		/// As the duration of an MPEG file is calculated (filesize / bitrate), this overhead must be accounted for.
		/// This function determines the overhead and returns it as a modifier to the calculation in the form
		/// of a number less than 1 which the result of (filesize / bitrate) is multiplied by.
		/// This function is adapted from James Heinrich's getID3 suite (www.getid3.org).
		/// </remarks>
		private static double MPEGsystemNonOverheadPercentage(int VideoBitrate, int AudioBitrate)
		{
			double OverheadPercentage = 0.0;

			int audioBitrate = Math.Max(Math.Min(AudioBitrate / 1000,   384), 32); // limit to range of 32kbps - 384kbps (should be only legal bitrates, but maybe VBR?)
			int videoBitrate = Math.Max(Math.Min(VideoBitrate / 1000, 10000), 10); // limit to range of 10kbps -  10Mbps (beyond that curves flatten anyways, no big loss)

			double[][] OverheadMultiplierByBitrate = new double[12][];
			OverheadMultiplierByBitrate[0]  = new double[] { 32, 0, 0.9676287944368530, 0.9802276264360310, 0.9844916183244460, 0.9852821845179940 };
			OverheadMultiplierByBitrate[1]  = new double[] { 48, 0, 0.9779100089209830, 0.9787770035359320, 0.9846738664076130, 0.9852683013799960 };
			OverheadMultiplierByBitrate[2]  = new double[] { 56, 0, 0.9731249855367600, 0.9776624308938040, 0.9832606361852130, 0.9843922606633340 };
			OverheadMultiplierByBitrate[3]  = new double[] { 64, 0, 0.9755642683275760, 0.9795256705493390, 0.9836573009193170, 0.9851122539404470 };
			OverheadMultiplierByBitrate[4]  = new double[] { 96, 0, 0.9788025247497290, 0.9798553314148700, 0.9822956869792560, 0.9834815119124690 };
			OverheadMultiplierByBitrate[5]  = new double[] { 128, 0, 0.9816940050925480, 0.9821675936072120, 0.9829756927470870, 0.9839763420152050 };
			OverheadMultiplierByBitrate[6]  = new double[] { 160, 0, 0.9825894094561180, 0.9820913399073960, 0.9823907143253970, 0.9832821783651570 };
			OverheadMultiplierByBitrate[7]  = new double[] { 192, 0, 0.9832038474336260, 0.9825731694317960, 0.9821028622712400, 0.9828262076447620 };
			OverheadMultiplierByBitrate[8]  = new double[] { 224, 0, 0.9836516298538770, 0.9824718601823890, 0.9818302180625380, 0.9823735101626480 };
			OverheadMultiplierByBitrate[9]  = new double[] { 256, 0, 0.9845863022094920, 0.9837229411967540, 0.9824521662210830, 0.9828645172100790 };
			OverheadMultiplierByBitrate[10] = new double[] { 320, 0, 0.9849565280263180, 0.9837683142805110, 0.9822885275960400, 0.9824424382727190 };
			OverheadMultiplierByBitrate[11] = new double[] { 384, 0, 0.9856094774357600, 0.9844573394432720, 0.9825970399837330, 0.9824673808303890 };

			double BitrateToUseMin = 0;
			double BitrateToUseMax = 0;
			double previousBitrate = 32;
			double[] key;
			for(int i=0;i<OverheadMultiplierByBitrate.Length;i++)
			{
				key = OverheadMultiplierByBitrate[i];
				if (audioBitrate >= previousBitrate)
				{
					BitrateToUseMin = i-1;
				}
				if (audioBitrate < key[0])
				{
					BitrateToUseMax = i;
					break;
				}
				previousBitrate = key[0];
			}
			if (BitrateToUseMin < 0) BitrateToUseMin = 0;
			double FactorA = (OverheadMultiplierByBitrate[(int)BitrateToUseMax][0] - audioBitrate) / (OverheadMultiplierByBitrate[(int)BitrateToUseMax][0] - OverheadMultiplierByBitrate[(int)BitrateToUseMin][0]);

			double VideoBitrateLog10 = Math.Log10(videoBitrate);
			double VideoFactorMin1 = OverheadMultiplierByBitrate[(int)BitrateToUseMin][(int)Math.Floor(VideoBitrateLog10)+1];
			double VideoFactorMin2 = OverheadMultiplierByBitrate[(int)BitrateToUseMax][(int)Math.Floor(VideoBitrateLog10)+1];
			double VideoFactorMax1 = OverheadMultiplierByBitrate[(int)BitrateToUseMin][(int)Math.Ceiling(VideoBitrateLog10)+1];
			double VideoFactorMax2 = OverheadMultiplierByBitrate[(int)BitrateToUseMax][(int)Math.Ceiling(VideoBitrateLog10)+1];
			double FactorV = VideoBitrateLog10 - Math.Floor(VideoBitrateLog10);

			OverheadPercentage  = VideoFactorMin1 *      FactorA  *      FactorV;
			OverheadPercentage += VideoFactorMin2 * (1 - FactorA) *      FactorV;
			OverheadPercentage += VideoFactorMax1 *      FactorA  * (1 - FactorV);
			OverheadPercentage += VideoFactorMax2 * (1 - FactorA) * (1 - FactorV);

			return OverheadPercentage;
		}
		#endregion
	}
}