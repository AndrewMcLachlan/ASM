using System;

namespace Asm.Media
{
    /// <summary>
    /// BitmapCompressionMethod.
    /// </summary>
    public enum BitmapCompressionMethod
    {
        /// <summary>
        /// BI_RGB
        /// </summary>
        BIRgb = 0,
        /// <summary>
        /// BI_RLE8
        /// </summary>
        BIRle8 = 1,
        /// <summary>
        /// BI_RLE4
        /// </summary>
        BIRle4 = 2,
        /// <summary>
        /// BI_BITFIELDS
        /// </summary>
        /// <remarks>
        /// Also Huffman 1D compression for BITMAPCOREHEADER2.
        /// </remarks>
        BIBitfields = 3, 
        /// <summary>
        /// BI_JPEG
        /// </summary>
        /// <remarks>
        /// Also RLE-24 compression for BITMAPCOREHEADER2
        /// </remarks>
        BIJpeg = 4,
        /// <summary>
        /// BI_PNG
        /// </summary>
        BIPng = 5,
    }
}