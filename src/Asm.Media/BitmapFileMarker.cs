using System.Runtime.InteropServices;

namespace Asm.Media;

/// <summary>
/// The file marker bytes for a Bitmap file.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public sealed class BitmapFileMarker
{
    /// <summary>
    /// The Default bitmap file marker.
    /// </summary>
    public static byte[] DefaultFileMarker() { return new byte[] { 0x42, 0x4D }; }

    /// <summary>
    /// The file marker.
    /// </summary>
    public short FileMarker { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapFileMarker"/> class.
    /// </summary>
    public BitmapFileMarker()
    {
        FileMarker = BitConverter.ToInt16(DefaultFileMarker(), 0);
    }
}
