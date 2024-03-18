using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Asm.Media;

/// <summary>
/// The header of a bitmap file.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public sealed class BitmapHeader
{
    /// <summary>
    /// The size in bytes of this header.
    /// </summary>
    /// <remarks>
    /// Note that this size is not necessarily the same size as a the object in memory.
    /// </remarks>
    public const int HeaderSize = 14;

    /// <summary>
    /// The file size.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Reserved value.
    /// </summary>
    public short Reserved1 { get; set; }

    /// <summary>
    /// Reserved value.
    /// </summary>
    public short Reserved2 { get; set; }

    /// <summary>
    /// Image offset.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Initialzes a new instance of the <see cref="BitmapHeader"/> struct.
    /// </summary>
    public BitmapHeader()
    {
    }

    internal void Write(System.IO.MemoryStream mem)
    {
        //mem.Write(BitConverter.GetBytes(FileMarker), 0, 2);
        mem.Write(BitConverter.GetBytes(Size), 0, 4);
        mem.Write(BitConverter.GetBytes(Reserved1), 0, 2);
        mem.Write(BitConverter.GetBytes(Reserved2), 0, 2);
        mem.Write(BitConverter.GetBytes(Offset), 0, 4);
    }
}
