namespace Asm.Media;

/// <summary>
/// Represents a Real Media movie file.
/// </summary>
public class RealMedia : MediaFile
{
    #region Fields
    private uint maxBitrate;  //b/s
    private uint avgBitrate;  //b/s
    private uint maxBitrateVideo;
    private uint avgBitrateVideo;
    private uint maxBitrateAudio;
    private uint avgBitrateAudio;
    #endregion

    #region Properties
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new instance of the <see cref="RealMedia"/> class.
    /// </summary>
    /// <param name="fileName">The name and path of the file to read.</param>
    public RealMedia(string fileName) : base(fileName)
    {
        this.MediaType = "Real Media";
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

        ulong bytesRead = Convert.ToUInt64(start + headerOffset);

        while (data.BaseStream.Position < Convert.ToInt64((start + length)) - headerOffset)
        {
            //Declare and initialize real media object
            Header rm = new Header(parent);

            //Read ID and size
            ByteArray objectId = new ByteArray(4, Endian.BigEndian);
            ByteArray rmSize = new ByteArray(4, Endian.BigEndian);
            ByteArray rmVersion = new ByteArray(2, Endian.BigEndian);

            data.Read(objectId.GetBytes(), 0, 4);
            data.Read(rmSize.GetBytes(), 0, 4);
            data.Read(rmVersion.GetBytes(), 0, 2);

            rm.MediaType = objectId.ToString();
            rm.Size = rmSize.ToUInt32();
            rm.Version = (short)rmVersion.ToUInt16();

            //Decide what to do
            switch (rm.MediaType)
            {
                case "PROP":
                    GetBitrate(rm, data, bytesRead, out this.maxBitrate, out this.avgBitrate);
                    GetDuration(rm, data, bytesRead);
                    this.Bitrate = this.maxBitrate;
                    break;
                case "MDPR":
                    switch (GetStreamType(data, bytesRead))
                    {
                        case "Video Stream":
                            GetBitrate(rm, data, bytesRead, out this.maxBitrateVideo, out this.avgBitrateVideo);
                            GetHeightWidth(data, bytesRead);
                            break;
                        case "Audio Stream":
                            GetBitrate(rm, data, bytesRead, out this.maxBitrateAudio, out this.avgBitrateAudio);
                            break;
                    }
                    break;
            }

            //Push the bytes read variable along.
            bytesRead += (ulong)rm.Size;
            data.BaseStream.Position = Convert.ToInt64(bytesRead);
        }
    }
    #endregion

    #region Private Methods
    private static string GetStreamType(BinaryReader Data, ulong bytesRead)
    {
        long tempPosition = Data.BaseStream.Position;

        Data.BaseStream.Position = Convert.ToInt64(bytesRead) + 40;

        byte streamNameSize = Data.ReadByte();
        ByteArray streamName = new ByteArray(streamNameSize, Endian.BigEndian);
        Data.Read(streamName.GetBytes(), 0, streamNameSize);

        string name = streamName.ToString();

        Data.BaseStream.Position = tempPosition;

        return name;
    }


    private void GetDuration(Header rm, BinaryReader Data, ulong bytesRead)
    {
        long tempPosition = Data.BaseStream.Position;
        int offset = 30;

        switch (rm.MediaType)
        {
            case "PROP":
                offset = 30;
                break;
            case "MDPR":
                offset = 36;
                break;
        }

        Data.BaseStream.Position = Convert.ToInt64(bytesRead) + offset;

        ByteArray duration = new ByteArray(4, Endian.BigEndian);

        Data.Read(duration.GetBytes(), 0, 4);

        Duration = duration.ToUInt32() * 0.001;

        Data.BaseStream.Position = tempPosition;
    }

    private static void GetBitrate(Header rm, BinaryReader Data, ulong bytesRead, out uint MaxBitrate, out uint AvgBitrate)
    {
        long tempPosition = Data.BaseStream.Position;
        int offset = 10;

        switch (rm.MediaType)
        {
            case "PROP":
                offset = 10;
                break;
            case "MDPR":
                offset = 12;
                break;
        }

        Data.BaseStream.Position = Convert.ToInt64(bytesRead) + offset;

        ByteArray maxBitrate = new ByteArray(4, Endian.BigEndian);
        ByteArray avgBitrate = new ByteArray(4, Endian.BigEndian);

        Data.Read(maxBitrate.GetBytes(), 0, 4);
        Data.Read(avgBitrate.GetBytes(), 0, 4);

        MaxBitrate = maxBitrate.ToUInt32();
        AvgBitrate = avgBitrate.ToUInt32();

        Data.BaseStream.Position = tempPosition;
    }

    private void GetHeightWidth(BinaryReader data, ulong bytesRead)
    {
        long tempPosition = data.BaseStream.Position;

        data.BaseStream.Position = Convert.ToInt64(bytesRead) + 40;

        //Get the stream name size field
        byte streamNameSize = data.ReadByte();
        data.BaseStream.Position += streamNameSize;

        //Get the mime type size field
        byte mimeTypeSize = data.ReadByte();
        data.BaseStream.Position += mimeTypeSize;

        //Get the type specific size field
        ByteArray typeSpecificLength = new ByteArray(4, Endian.BigEndian);
        data.Read(typeSpecificLength.GetBytes(), 0, 4);

        ByteArray typeSpecificData = new ByteArray(typeSpecificLength.ToInt32(), Endian.BigEndian);
        data.Read(typeSpecificData.GetBytes(), 0, typeSpecificLength.ToInt32());

        ByteArray height = typeSpecificData.Copy(12, 2);
        ByteArray width = typeSpecificData.Copy(14, 2);

        Height = height.ToUInt16();
        Width = width.ToUInt16();

        data.BaseStream.Position = tempPosition;
    }
    #endregion
}