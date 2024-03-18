using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Asm.Media;

/// <summary>
/// The info header for a Bitmap file.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public class BitmapInfoHeader
{
    #region Properties
    /// <summary>
    /// The size of the header.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Image width.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Image height.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Colour planes.
    /// </summary>
    /// <remarks>
    /// Must be 1.
    /// </remarks>
    public short ColourPlanes { get; set; }

    /// <summary>
    /// Image colour depth.
    /// </summary>
    public short ColourDepth { get; set; }

    /// <summary>
    /// One of the <see cref="BitmapCompressionMethod"/> values.
    /// </summary>
    public int CompressionMethod { get; set; }

    /// <summary>
    /// The size in bytes of the image data.
    /// </summary>
    public int RawImageSize { get; set; }

    /// <summary>
    /// The horizontal resolution.
    /// </summary>
    public int HorizontalResolution { get; set; }

    /// <summary>
    /// The vertical resolution.
    /// </summary>
    public int VerticalResolution { get; set; }

    /// <summary>
    /// The number of colours used.
    /// </summary>
    public int Colours { get; set; }

    /// <summary>
    /// The number of "important" colours.
    /// </summary>
    public int ImportantColours { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapInfoHeader" /> struct.
    /// </summary>
    public BitmapInfoHeader()
    {
        Size = 40;
    }
    #endregion

    #region Internal Methods
    internal void Write(System.IO.MemoryStream mem)
    {
        mem.Write(BitConverter.GetBytes(Size), 0, 4);
        mem.Write(BitConverter.GetBytes(Width), 0, 4);
        mem.Write(BitConverter.GetBytes(Height), 0, 4);
        mem.Write(BitConverter.GetBytes(ColourPlanes), 0, 2);
        mem.Write(BitConverter.GetBytes(ColourDepth), 0, 2);
        mem.Write(BitConverter.GetBytes(CompressionMethod), 0, 4);
        mem.Write(BitConverter.GetBytes(RawImageSize), 0, 4);
        mem.Write(BitConverter.GetBytes(HorizontalResolution), 0, 4);
        mem.Write(BitConverter.GetBytes(VerticalResolution), 0, 4);
        mem.Write(BitConverter.GetBytes(Colours), 0, 4);
        mem.Write(BitConverter.GetBytes(ImportantColours), 0, 4);
    }
    #endregion
}
