using AscentMedia.Utils;
using System;
using System.Collections;
using System.IO;

namespace Asm.Media
{
	/// <summary>
	/// Represents a Windows Media movie file.
	/// </summary>
	public class WindowsMedia : MediaFile
	{
		#region Fields
		private ArrayList bitrates;
		private double maxBitrate;

		//GUID values used by Windows Media
		private const string ASF_Header_Object					   = "75b22630-668e-11cf-a6d9-00aa0062ce6c";
		private const string ASF_Data_Object					   = "75b22636-668e-11cf-a6d9-00aa0062ce6c";
		private const string ASF_Simple_Index_Object			   = "33000890-e5b1-11cf-89f4-00a0c90349cb";
		private const string ASF_Index_Object					   = "d6e229d3-35da-11d1-9034-00a0c90349be";
		private const string ASF_Media_Object_Index_Object		   = "feb103f8-12ad-4c64-840f-2a1d2f7ad48c";
		private const string ASF_Timecode_Index_Object			   = "3cb73fd0-0c4a-4803-953d-edf7b6228f0c";
		private const string ASF_File_Properties_Object			   = "8cabdca1-a947-11cf-8ee4-00c00c205365";
		private const string ASF_Stream_Properties_Object		   = "b7dc0791-a9b7-11cf-8ee6-00c00c205365";
		private const string ASF_Audio_Media					   = "f8699e40-5b4d-11cf-a8fd-00805f5c442b";
		private const string ASF_Video_Media					   = "bc19efc0-5b4d-11cf-a8fd-00805f5c442b";
		private const string ASF_Stream_Bitrate_Properties_Object  = "7bf875ce-468d-11d1-8d82-006097c9a2b2";
		private const string ASF_Header_Extension_Object		   = "5fbf03b5-a92e-11cf-8ee3-00c00c205365";
		private const string ASF_Extended_Stream_Properties_Object = "14e6a5cb-c672-4332-8399-a96952065b5a";
		#endregion

		#region Properties
		/// <summary>
		/// The total bitrate (audio and video streams) of the file in b/s.
		/// </summary>
		/// <remarks>
		/// To get the bitrate in Kb/s, divide this value by the KilobitConvertor constant.
		/// </remarks>
		public override double Bitrate
		{
			get
			{
				if (Bitrate == 0)
				{
					return maxBitrate;
				}
				else
				{
					return Bitrate;
				}
			}
		}

		/// <summary>
		/// Bitrates for individual streams (including overhead).
		/// </summary>
		public ArrayList Bitrates
		{
			get
			{
				return bitrates;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of the <see cref="WindowsMedia"/> class.
		/// </summary>
        /// <param name="fileName">The name and path of the file to read.</param>
		public WindowsMedia(string fileName) : base(fileName)
		{
			this.MediaType = "Advanced Systems Format / Windows Media";
			this.bitrates = new ArrayList();
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

			data.BaseStream.Position = start + headerOffset;

            long bytesRead = start + headerOffset;
			short offset = 24;

			while (data.BaseStream.Position < Convert.ToInt64((start + length))-headerOffset)
			{
				Header asf = new Header(parent);

				ByteArray guid = new ByteArray(16);
				ByteArray asfSize = new ByteArray(8, Endian.LittleEndian);

                data.Read(guid.GetBytes(), 0, 16);
                data.Read(asfSize.GetBytes(), 0, 8);

				asf.Size = (long) asfSize.ToInt64();
				asf.MediaType = guid.ToGuid().ToString();

				switch (asf.MediaType)
				{
					case ASF_Header_Object :
						offset = 30;
						GetMetadata(asf, data, bytesRead, asf.Size, offset);
						break;
					case ASF_Header_Extension_Object :
						offset = 46;
						GetMetadata(asf, data, bytesRead, asf.Size, offset);
						break;
					case ASF_File_Properties_Object :
						GetDuration(asf, data, bytesRead);
						GetMaxBitrate(asf, data, bytesRead, out this.maxBitrate);
						break;
					case ASF_Stream_Properties_Object :
						GetHeightWidth(data, bytesRead);
						break;
					case ASF_Stream_Bitrate_Properties_Object :
						bitrates.AddRange(GetBitrateRecords(data, bytesRead));
						break;
					case ASF_Extended_Stream_Properties_Object :
						double tempBitrate;
						GetStreamBitrate(data, bytesRead, out tempBitrate);
						this.Bitrate += tempBitrate;
						break;
				}

				bytesRead += (long) asf.Size;
				data.BaseStream.Position = Convert.ToInt64(bytesRead);

				if (parent == null)
				{
					Headers.Add(asf);
				}
				else
				{
					parent.SubHeaders.Add(asf);
				}
			}
		}
        #endregion

        #region Private Methods
        private void GetDuration(Header Asf, BinaryReader Data, long bytesRead)
		{
			long tempPosition = Data.BaseStream.Position;

			Data.BaseStream.Position = Convert.ToInt64(bytesRead) + Convert.ToInt64(Asf.Size) - 40;

			ByteArray duration = new ByteArray(8, Endian.LittleEndian);

            Data.Read(duration.GetBytes(), 0, 8);

			Duration = (long) duration.ToUInt64() * 0.0000001;

			Data.BaseStream.Position = tempPosition;

		}

		private static void GetMaxBitrate(Header Asf, BinaryReader Data, long bytesRead, out double bitrate)
		{
			long tempPosition = Data.BaseStream.Position;

			Data.BaseStream.Position = Convert.ToInt64(bytesRead) + Convert.ToInt64(Asf.Size) - 4;

			ByteArray bitrateArray = new ByteArray(4, Endian.LittleEndian);

			Data.Read(bitrateArray.GetBytes(), 0, 4);

			bitrate = bitrateArray.ToUInt32();

			Data.BaseStream.Position = tempPosition;
		}

		private static void GetStreamBitrate(BinaryReader Data, long bytesRead, out double bitrate)
		{
			long tempPosition = Data.BaseStream.Position;

			Data.BaseStream.Position = Convert.ToInt64(bytesRead) + 40;

			ByteArray bitrateArray = new ByteArray(4, Endian.LittleEndian);

            Data.Read(bitrateArray.GetBytes(), 0, 4);

			bitrate = bitrateArray.ToUInt32();

			Data.BaseStream.Position = tempPosition;
		}

		private static ArrayList GetBitrateRecords(BinaryReader Data, long BytesRead)
		{
			long tempPosition = Data.BaseStream.Position;

			Data.BaseStream.Position = Convert.ToInt64(BytesRead) + 24;

			ByteArray bitrateRecordCount = new ByteArray(2, Endian.LittleEndian);
            Data.Read(bitrateRecordCount.GetBytes(), 0, 2);

			ushort recordCount = bitrateRecordCount.ToUInt16();

			ArrayList temp = new ArrayList();

			ByteArray avgBitrate = new ByteArray(4, Endian.LittleEndian);

			for (int i=0;i<recordCount;i++)
			{
				//Skip the flags
				Data.BaseStream.Position += 2;
                Data.Read(avgBitrate.GetBytes(), 0, 4);
				temp.Add(avgBitrate.ToUInt32());
			}

			Data.BaseStream.Position = tempPosition;

			return temp;
		}
		
		private void GetHeightWidth(BinaryReader Data, long BytesRead)
		{
			long tempPosition = Data.BaseStream.Position;

			Data.BaseStream.Position = Convert.ToInt64(BytesRead) + 24;

			ByteArray streamType = new ByteArray(16);

            Data.Read(streamType.GetBytes(), 0, 16);

			if (streamType.ToGuid().ToString() != ASF_Video_Media)
			{
				this.Height = 0;
				this.Width = 0;
				return;
			}

			Data.BaseStream.Position += 38;

			ByteArray height = new ByteArray(4, Endian.LittleEndian);
			ByteArray width = new ByteArray(4, Endian.LittleEndian);

            Data.Read(height.GetBytes(), 0, 4);
            Data.Read(width.GetBytes(), 0, 4);

			this.Height = (int) height.ToUInt32();
			this.Width = (int) width.ToUInt32();

			Data.BaseStream.Position = tempPosition;
		}
		#endregion
	}
}