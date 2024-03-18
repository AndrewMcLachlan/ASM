using System.Runtime.InteropServices;

namespace Asm.Drawing;

[StructLayout(LayoutKind.Sequential)]
internal sealed class IconHeader
{
    public short FileMarker { get; private set; }
    public short FileType { get; set; }
    public short NumberOfImages { get; set; }

    public IconHeader()
    {
        FileMarker = 0;
    }

    public void Write(Stream stream)
    {
        int headerSize = Marshal.SizeOf(typeof(IconHeader));

        IntPtr headerPtr = Marshal.AllocHGlobal(headerSize);

        try
        {
            Marshal.StructureToPtr(this, headerPtr, true);

            byte[] buffer = new byte[headerSize];
            Marshal.Copy(headerPtr, buffer, 0, headerSize);
            stream.Write(buffer, 0, headerSize);
        }
        finally
        {
            Marshal.FreeHGlobal(headerPtr);
        }
    }
}
