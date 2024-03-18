using System;
using System.Collections;
using System.IO;
using Asm.Media.Utils;

namespace Asm.Media;

/// <summary>
/// Represents a Quicktime movie file.
/// </summary>
public class Quicktime : MediaFile
{
    #region Fields
    /// <summary>
    ///
    /// </summary>
    public new const int KilobitConverter = 1024;
    #endregion

    #region Properties
    /// <summary>
    /// The total bitrate (audio and video streams) of the file in Kb/s.
    /// </summary>
    public override double Bitrate
    {
        get
        {
            //return (((movieSize / 1024.0)* 8.0) / duration);
            return ((MovieSize * 8.0) / Duration);
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new instance of the <see cref="Quicktime"/> class.
    /// </summary>
    /// <param name="fileName">The name and path of the file to read.</param>
    public Quicktime(string fileName) : base(fileName)
    {
        this.MediaType = "Quicktime";
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
        short offset = 0;

        while (data.BaseStream.Position < Convert.ToInt64((start + length)) - headerOffset)
        {
            Header atom = new Header(parent);

            ByteArray atomSize = new ByteArray(4, Endian.BigEndian);
            ByteArray atomType = new ByteArray(4, Endian.BigEndian);

            data.Read(atomSize.GetBytes(), 0, 4);
            data.Read(atomType.GetBytes(), 0, 4);

            offset = 8;

            atom.Size = atomSize.ToUInt32();
            atom.MediaType = atomType.ToString();

            if (atom.Size == 1)
            {
                ByteArray atomExtendedSize = new ByteArray(8, Endian.BigEndian);
                data.Read(atomExtendedSize.GetBytes(), 0, 8);
                atom.Size = (long)atomExtendedSize.ToUInt64();

                offset += 8;
            }
            else if (atom.Size == 0)
            {
                Headers.Add(atom);
                break;
            }

            //Set the header size
            atom.HeaderSize = (short)offset;

            if (atom.MediaType == "moov" ||
                atom.MediaType == "clip" ||
                atom.MediaType == "udta" ||
                atom.MediaType == "trak" ||
                atom.MediaType == "matt" ||
                atom.MediaType == "edts" ||
                atom.MediaType == "mdia" ||
                atom.MediaType == "minf" ||
                atom.MediaType == "dinf" ||
                atom.MediaType == "stbl" ||
                atom.MediaType == "mdat")
            {
                GetMetadata(atom, data, bytesRead, atom.Size, offset);
            }

            if (atom.MediaType == "tkhd" && this.Width == 0)
            {
                GetHeightWidth(atom, data, bytesRead);
            }
            else if (atom.MediaType == "mvhd")
            {
                GetDuration(data, bytesRead);
            }
            else if (atom.MediaType == "mdat")
            {
                this.MovieSize = atom.Size - offset;
                foreach (Header subAtom in atom.SubHeaders)
                {
                    this.MovieSize -= subAtom.Size;
                }
            }

            bytesRead += atom.Size;
            data.BaseStream.Position = Convert.ToInt64(bytesRead);

            if (parent == null)
            {
                this.Headers.Add(atom);
            }
            else
            {
                parent.SubHeaders.Add(atom);
            }
        }
    }
    #endregion

    #region Private Methods
    private void GetDuration(BinaryReader Data, long bytesRead)
    {
        long tempPosition = Data.BaseStream.Position;

        Data.BaseStream.Position = Convert.ToInt64(bytesRead) + 20;

        ByteArray timescale = new ByteArray(4, Endian.BigEndian);
        ByteArray duration = new ByteArray(4, Endian.BigEndian);

        Data.Read(timescale.GetBytes(), 0, 4);
        Data.Read(duration.GetBytes(), 0, 4);

        uint iTimescale = timescale.ToUInt32();
        uint iDuration = duration.ToUInt32();

        this.Duration = Convert.ToSingle(iDuration) / Convert.ToSingle(iTimescale);

        Data.BaseStream.Position = tempPosition;
    }

    private void GetHeightWidth(Header atom, BinaryReader Data, long bytesRead)
    {
        long tempPosition = Data.BaseStream.Position;

        Data.BaseStream.Position = Convert.ToInt64(bytesRead) + Convert.ToInt64(atom.Size) - 8;

        ByteArray height = new ByteArray(2, Endian.BigEndian);
        ByteArray width = new ByteArray(2, Endian.BigEndian);

        Data.Read(height.GetBytes(), 0, 2);
        Data.BaseStream.Position += 2;
        Data.Read(width.GetBytes(), 0, 2);

        this.Height = (short)height.ToUInt16();
        this.Width = (short)width.ToUInt16();

        Data.BaseStream.Position = tempPosition;
    }

    #endregion
}
