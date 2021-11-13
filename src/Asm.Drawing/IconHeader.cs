using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Asm.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class IconHeader
    {
        public Int16 FileMarker { get; private set; }
        public Int16 FileType { get; set; }
        public Int16 NumberOfImages { get; set; }

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
}
