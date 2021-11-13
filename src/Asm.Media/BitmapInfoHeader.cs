using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Asm.Media
{
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
        public Int32 Size { get; set; }
        
        /// <summary>
        /// Image width.
        /// </summary>
        public Int32 Width { get; set; }
        
        /// <summary>
        /// Image height.
        /// </summary>
        public Int32 Height { get; set; }
        
        /// <summary>
        /// Colour planes.
        /// </summary>
        /// <remarks>
        /// Must be 1.
        /// </remarks>
        public Int16 ColourPlanes { get; set; }
        
        /// <summary>
        /// Image colour depth.
        /// </summary>
        public Int16 ColourDepth { get; set; }
        
        /// <summary>
        /// One of the <see cref="BitmapCompressionMethod"/> values.
        /// </summary>
        public Int32 CompressionMethod { get; set; }
        
        /// <summary>
        /// The size in bytes of the image data.
        /// </summary>
        public Int32 RawImageSize { get; set; }
        
        /// <summary>
        /// The horizontal resolution.
        /// </summary>
        public Int32 HorizontalResolution { get; set; }
        
        /// <summary>
        /// The vertical resolution.
        /// </summary>
        public Int32 VerticalResolution { get; set; }
        
        /// <summary>
        /// The number of colours used.
        /// </summary>
        public Int32 Colours { get; set; }
        
        /// <summary>
        /// The number of "important" colours.
        /// </summary>
        public Int32 ImportantColours { get; set; }
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
}
