using System.Runtime.InteropServices;

namespace Asm.Drawing;

[StructLayout(LayoutKind.Sequential)]
internal class IconDirEntry
{
    public byte Width { get; set; }
    public byte Height { get; set; }
    public byte Colours { get; set; }
    public byte Reserved { get; set; }
    public short Custom1 { get; set; }
    public short Custom2 { get; set; }
    public int Size { get; set; }
    public int Offset { get; set; }

    public virtual void WriteHeader(Stream stream, int offset)
    {
        Offset = offset;

        int dirSize = Marshal.SizeOf(typeof(IconDirEntry));

        IntPtr dirPtr = Marshal.AllocHGlobal(dirSize);

        try
        {
            Marshal.StructureToPtr(this, dirPtr, true);

            byte[] buffer = new byte[dirSize];
            Marshal.Copy(dirPtr, buffer, 0, dirSize);
            stream.Write(buffer, 0, dirSize);
        }
        finally
        {
            Marshal.FreeHGlobal(dirPtr);
        }
    }

    public virtual void WriteHeader(Stream str)
    {
        WriteHeader(str, Offset);
    }
}
