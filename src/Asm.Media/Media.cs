using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Asm.Media;

/// <summary>
/// Generic class that represents a movie file.
/// </summary>
[Serializable]
public abstract class MediaFile
{
    #region Fields
    private string type = "Unknown";
    private Collection<Header> headers;
    private long size;
    private long movieSize;
    private string fileName;
    private int height;
    private int width;
    private double duration;
    private double bitrate;
    private double audioBitrate;
    private double videoBitrate;
    /// <summary>
    /// Value for conversion from bits to kilobits.
    /// </summary>
    public const int KilobitConverter = 1000;
    #endregion

    #region Properties
    /// <summary>
    /// The type of media file (Quicktime, Real, MPEG 1, MPEG 2, MPEG 4 etc).
    /// </summary>
    public string MediaType
    {
        get { return type; }
        protected set { type = value; }
    }

    /// <summary>
    /// The size of the file in bytes.
    /// </summary>
    public virtual long Size
    {
        get
        {
            return size;
        }
    }

    /// <summary>
    /// The top-level meta data objects contained in this movie.
    /// </summary>
    public virtual Collection<Header> Headers
    {
        get
        {
            return headers;
        }
    }

    /// <summary>
    /// The height of the movie frame in pixels.
    /// </summary>
    public virtual int Height
    {
        get { return height; }
        protected set { height = value; }
    }

    /// <summary>
    /// The width of the movie frame in pixels.
    /// </summary>
    public virtual int Width
    {
        get { return width; }
        protected set { width = value; }
    }

    /// <summary>
    /// The size of the movie data in bytes.
    /// </summary>
    public virtual long MovieSize
    {
        get { return movieSize; }
        protected set { movieSize = value; }
    }

    /// <summary>
    ///The duration of the movie in seconds.
    /// </summary>
    public virtual double Duration
    {
        get { return duration; }
        protected set { duration = value; }
    }

    /// <summary>
    /// The total bitrate (audio and video streams) of the file in b/s.
    /// </summary>
    /// <remarks>
    /// This property uses the standard ratio of 1024 bytes per kilobyte.
    /// If the movie format uses a different ratio (ie 1000 b/Kb) then this property should be overidden.
    /// </remarks>
    public virtual double Bitrate
    {
        get { return bitrate; } // / 1024;
        protected set { bitrate = value; }
    }

    /// <summary>
    /// The bitrate of the video stream in b/s.
    /// </summary>
    public virtual double VideoBitrate
    {
        get { return videoBitrate; }
        protected set { videoBitrate = value; }
    }

    /// <summary>
    /// The bitrate of the audio stream in b/s.
    /// </summary>
    public virtual double AudioBitrate
    {
        get { return audioBitrate; }
        protected set { audioBitrate = value; }
    }

    /// <summary>
    /// The name of the file.
    /// </summary>
    protected virtual string FileName
    {
        get { return fileName; }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new instance of the <see cref="Media"/> class.
    /// </summary>
    /// <param name="fileName">The name and path of the file.</param>
    protected MediaFile(string fileName)
    {
        this.fileName = fileName;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Read the header information.
    /// </summary>
    /// <remarks>
    /// Note that calling this method on the base class (Media) will result in an empty instance of the class.
    /// </remarks>
    public virtual void Read()
    {
        try
        {
            using (BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read)))
            {
                this.size = br.BaseStream.Length;

                headers = new Collection<Header>();

                try
                {
                    GetMetadata(null, br, 0, br.BaseStream.Length, 0);
                }
                catch (Exception ex)
                {
                    throw new IOException("Unrecognised file type.", ex);
                }
            }
        }
        catch (System.IO.IOException ex)
        {
            throw new IOException("Unable to open file.", ex);
        }
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
    protected abstract void GetMetadata(Header parent, BinaryReader data, long start, long length, short headerOffset);
    #endregion
}
