using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Asm.Media;

namespace Asm.Drawing.Imaging;

/// <summary>
/// Helper class for <see cref="System.Drawing.Imaging.ImageFormat"/>
/// </summary>
public static class ImageFormatHelper
{
    /// <summary>
    /// Gets an <see cref="System.Drawing.Imaging.ImageFormat"/> from a stream.
    /// </summary>
    /// <remarks>Currently only supports BMP and PNG files.</remarks>
    /// <param name="stream">The image data stream.</param>
    /// <returns>The <see cref="System.Drawing.Imaging.ImageFormat"/> of the image in the stream.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
    public static ImageFormat GetImageFormat(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException("stream");

        byte[] buffer = new byte[8];

        stream.Read(buffer, 0, unchecked((int)Math.Min(buffer.Length, stream.Length)));

        return GetImageFormat(buffer);
    }

    /// <summary>
    /// Gets an <see cref="System.Drawing.Imaging.ImageFormat"/> from a byte array.
    /// </summary>
    /// <remarks>Currently only supports BMP and PNG files.</remarks>
    /// <param name="buffer">The image data array.</param>
    /// <returns>The <see cref="System.Drawing.Imaging.ImageFormat"/> of the image in the stream.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="buffer"/> is null.</exception>
    /// <exception cref="System.ArgumentException">Thrown if <paramref name="buffer"/> has a length of 0.</exception>
    public static ImageFormat GetImageFormat(byte[] buffer)
    {
        if (buffer == null) throw new ArgumentNullException("buffer");
        if (buffer.Length == 0) throw new ArgumentException("Buffer must have a length", "buffer");

        ImageFormat result;

        if (buffer.Length >= 8 && buffer[0] == 137 &&
                       buffer[1] == 80 &&
                       buffer[2] == 78 &&
                       buffer[3] == 71 &&
                       buffer[4] == 13 &&
                       buffer[5] == 10 &&
                       buffer[6] == 26 &&
                       buffer[7] == 10)
        {
            result = ImageFormat.Png;
        }

        else if (buffer.Length >= 2 && buffer[0] == 0x42 && buffer[1] == 0x4D)
        {
            result = ImageFormat.Bmp;
        }
        else
        {
            // TODO more formats
            result = ImageFormat.Bmp;
        }

        return result;
    }

    /// <summary>
    /// Gets an object that describes the format of the given image.
    /// </summary>
    /// <param name="image">The source image.</param>
    /// <returns>An <see cref="ImageFormatDescriptor"/> object that describes the image's format.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="image"/> is null.</exception>
    public static ImageFormatDescriptor GetImageFormatDescriptor(System.Drawing.Image image)
    {
        if (image == null) throw new ArgumentNullException("image");

        ImageFormatDescriptor result;

        if (image.RawFormat.Equals(ImageFormat.Bmp) || image.RawFormat.Equals(ImageFormat.MemoryBmp))
        {
            result = new ImageFormatDescriptor("Windows Bitmap", "BMP", new[] { "bmp" });
        }
        else if (image.RawFormat.Equals(ImageFormat.Gif))
        {
            result = new ImageFormatDescriptor("Grpahics Interchange Format", "GIF", new[] { "gif" });
        }
        else if (image.RawFormat.Equals(ImageFormat.Png))
        {
            result = new ImageFormatDescriptor("Portable Grpahics Format", "PNG", new[] { "png" });
        }
        else if (image.RawFormat.Equals(ImageFormat.Jpeg))
        {
            result = new ImageFormatDescriptor("Joint Photographic Experts Group", "JPEG", new[] { "jpg", "jpeg" });
        }
        else if (image.RawFormat.Equals(ImageFormat.Tiff))
        {
            result = new ImageFormatDescriptor("Tagged Image File Format", "TIFF", new[] { "tif", "tiff" });
        }
        else
        {
            throw new BadImageFormatException();
        }

        return result;
    }
}
