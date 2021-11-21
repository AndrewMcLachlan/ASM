using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.IO;
using System.Net.Mime;
using Asm.Media;
using System.Runtime.InteropServices;
using Asm;
using Asm.Drawing;

namespace System.Drawing
{
    /// <summary>
    /// Extension methods for the <see cref="System.Drawing.Image"/> class.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Gets the colour depth of the image.
        /// </summary>
        /// <param name="image">The <see cref="System.Drawing.Image"/>.</param>
        /// <returns>An int representing the colour depth.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="image"/> is null.</exception>
        public static int GetColourDepth(this Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            switch (image.PixelFormat)
            {
                case PixelFormat.DontCare:
                case PixelFormat.Max:
                case PixelFormat.Indexed:
                case PixelFormat.Alpha:
                case PixelFormat.Gdi:
                case PixelFormat.PAlpha:
                case PixelFormat.Extended:
                case PixelFormat.Canonical:
                    return 0;
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format48bppRgb:
                case PixelFormat.Format64bppPArgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format64bppArgb:
                    Regex replace = new("Format?([0-9]+)bpp.*");
                    Match match = replace.Match(image.PixelFormat.ToString());
                    return Int32.Parse(match.Groups[1].Value);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Saves the image as a JPEG file.
        /// </summary>
        /// <param name="image">The <see cref="System.Drawing.Image"/>.</param>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="compression">The compression to use from 0 (highest compression) to 100 (lowest compression).</param>
        public static void SaveJpeg(this Image image, Stream stream, long compression)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            using EncoderParameters eps = new(1);
            eps.Param[0] = new EncoderParameter(Encoder.Quality, compression);
            ImageCodecInfo ici = GetEncoderInfo(MediaTypeNames.Image.Jpeg);
            image.Save(stream, ici, eps);
        }

        /// <summary>
        /// Saves the image as a JPEG file.
        /// </summary>
        /// <param name="image">The <see cref="System.Drawing.Image"/>.</param>
        /// <param name="fileName">The name of the file to save to.</param>
        /// <param name="compression">The compression to use from 0 (highest compression) to 100 (lowest compression).</param>
        public static void SaveJpeg(this Image image, string fileName, long compression)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            using EncoderParameters eps = new(1);
            eps.Param[0] = new EncoderParameter(Encoder.Quality, compression);
            ImageCodecInfo ici = GetEncoderInfo(MediaTypeNames.Image.Jpeg);
            image.Save(fileName, ici, eps);
        }

        /// <summary>
        /// Saves the image as a 32 bit BMP.
        /// </summary>
        /// <param name="image">The <see cref="System.Drawing.Image"/> that this method extends.</param>
        /// <param name="fileName">The name of the to save to.</param>
        public static void SaveBmp32(this Image image, string fileName)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("Enter a valid file name", nameof(fileName));

            Bitmap bm = new(image);

            BitmapData bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            FileStream fs = File.Open(fileName, FileMode.OpenOrCreate);

            BitmapFileMarker bmfm = new();

            BitmapHeader bmh = new()
            {
                Offset = 54,
            };

            BitmapInfoHeader bmih = new()
            {
                ColourDepth = 32,
                ColourPlanes = 1,
                Colours = 0,
                CompressionMethod = (int)BitmapCompressionMethod.BIRgb,
                Height = image.Height,
                Width = image.Width,
                Size = 40,
                ImportantColours = 0,
                RawImageSize = image.Width * image.Height * 4,
                HorizontalResolution = (int)(image.HorizontalResolution * UnitConvert.InchesPerMetre),
                VerticalResolution = (int)(image.VerticalResolution * UnitConvert.InchesPerMetre),
            };

            IntPtr fileMarkerMem = Marshal.AllocHGlobal(2);
            Marshal.StructureToPtr(bmfm, fileMarkerMem, true);

            IntPtr bmpHeaderMem = Marshal.AllocHGlobal(12);
            Marshal.StructureToPtr(bmh, bmpHeaderMem, true);

            IntPtr dibHeaderMem = Marshal.AllocHGlobal(40);
            Marshal.StructureToPtr(bmih, dibHeaderMem, true);

            byte[] buffer = new byte[54];

            Marshal.Copy(fileMarkerMem, buffer, 0, 2);
            Marshal.Copy(bmpHeaderMem, buffer, 2, 14);
            Marshal.Copy(dibHeaderMem, buffer, 14, 40);

            Marshal.DestroyStructure(fileMarkerMem, typeof(BitmapFileMarker));
            Marshal.DestroyStructure(bmpHeaderMem, typeof(BitmapHeader));
            Marshal.DestroyStructure(dibHeaderMem, typeof(BitmapInfoHeader));

            fs.Write(buffer, 0, buffer.Length);

            buffer = new byte[bmd.Stride * bmd.Height];

            if (bmd.Stride > 0)
            {
                bm.UnlockBits(bmd);
                bm.RotateFlip(RotateFlipType.Rotate180FlipX);
                bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            }

            Marshal.Copy(bmd.Scan0, buffer, 0, buffer.Length);

            fs.Write(buffer, 0, buffer.Length);

            fs.Dispose();
            bm.UnlockBits(bmd);
            bm.Dispose();
        }

        /// <summary>
        /// Generates a thumbnail of an image, maintaining the aspect ratio.
        /// </summary>
        /// <param name="image">The image to thumbnail.</param>
        /// <param name="thumbMaxWidth">The thumbnail's maximum allowed width.</param>
        /// <param name="thumbMaxHeight">The thumbnail's maximum allowed height.</param>
        /// <returns>A thumbnail image.</returns>
        public static Image GetThumbnailImageMaintainAspectRatio(this Image image, int thumbMaxWidth, int thumbMaxHeight)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            int newHeight, newWidth;

            if (image.Height == image.Width)
            {
                newWidth = thumbMaxWidth;
                newHeight = thumbMaxHeight;
            }
            else if (image.Height > image.Width)
            {
                newHeight = thumbMaxHeight;
                double ratio = (double)image.Width / (double)image.Height;
                newWidth = (int)(newHeight * ratio);
            }
            else
            {
                newWidth = thumbMaxWidth;
                double ratio = (double)image.Height / (double)image.Width;
                newHeight = (int)(newWidth * ratio);
            }

            Bitmap thumbnail = new(newWidth, newHeight);

            // Maintain image metadata.
            foreach(PropertyItem pi in image.PropertyItems)
            {
                thumbnail.SetPropertyItem(pi);
            }

            try
            {
                using Graphics g = Graphics.FromImage(thumbnail);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));
            }
            catch
            {
                thumbnail.Dispose();
                throw;
            }

            return thumbnail;
        }

        /// <summary>
        /// Gets the date taken EXIF property item.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> instance that this method extends.</param>
        /// <returns>The date the image was taken or null if the property item is not available.</returns>
        public static DateTime? GetDateTaken(this Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            PropertyItem propItem;
            try
            {
                propItem = image.GetPropertyItem((int)ExifProperty.PropertyTagExifDTOrig);
            }
            catch (ArgumentException)
            {
                return null;
            }

            DateTime dateTaken;

            //Convert date taken metadata to a DateTime object
            string dateTakenString = System.Text.Encoding.UTF8.GetString(propItem.Value).Trim();
            string secondhalf = dateTakenString[dateTakenString.IndexOf(" ")..];
            string firsthalf = dateTakenString[..10];
            firsthalf = firsthalf.Replace(":", "-");
            dateTakenString = firsthalf + secondhalf;
            dateTaken = DateTime.Parse(dateTakenString);

            return dateTaken;
        }

        /// <summary>
        /// Gets the orientation EXIF property item.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> instance that this method extends.</param>
        /// <returns>The orientation of the image or <see cref="ExifOrientation.NotSet"/> if the property item is not available.</returns>
        public static ExifOrientation GetOrientation(this Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            PropertyItem propItem;
            try
            {
                propItem = image.GetPropertyItem((int)ExifProperty.PropertyTagOrientation);
                return (ExifOrientation)propItem.Value[0];
            }
            catch (InvalidCastException)
            {
                return ExifOrientation.NotSet;
            }
            catch (IndexOutOfRangeException)
            {
                return ExifOrientation.NotSet;
            }
            catch (ArgumentException)
            {
                return ExifOrientation.NotSet;
            }
        }

        /// <summary>
        /// Gets the orientation EXIF property item as an instruction on how to present the image at it's intended orientation.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> instance that this method extends.</param>
        /// <returns>The orientation of the image or <see cref="ExifOrientation.NotSet"/> if the property item is not available.</returns>
        public static RotateFlipType GetOrientationInstruction(this Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            PropertyItem? propItem;
            try
            {
                propItem = image.GetPropertyItem((int)ExifProperty.PropertyTagOrientation);

                if (propItem?.Value == null || propItem.Value.Length == 0) return RotateFlipType.RotateNoneFlipNone;

                return propItem.Value[0] switch
                {
                    2 => RotateFlipType.RotateNoneFlipX,
                    3 => RotateFlipType.Rotate180FlipNone,
                    4 => RotateFlipType.Rotate180FlipX,
                    5 => RotateFlipType.Rotate270FlipX,
                    6 => RotateFlipType.Rotate90FlipNone,
                    7 => RotateFlipType.Rotate90FlipX,
                    8 => RotateFlipType.Rotate270FlipNone,
                    _ => RotateFlipType.RotateNoneFlipNone,
                };
            }
            catch (IndexOutOfRangeException)
            {
                return RotateFlipType.RotateNoneFlipNone;
            }
            catch (ArgumentException)
            {
                return RotateFlipType.RotateNoneFlipNone;
            }
        }

        /// <summary>
        /// Gets an EXIF property item.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> instance that this method extends.</param>
        /// <param name="property">The EXIF property name.</param>
        /// <returns>The value of the property item or null if the property item is not available.</returns>
        public static string? GetProperty(this Image image, ExifProperty property)
        {
            PropertyItem? propItem;
            try
            {
                propItem = image.GetPropertyItem((int)property);
            }
            catch (ArgumentException)
            {
                return null;
            }

            if (propItem?.Value == null) return null;

            return System.Text.Encoding.UTF8.GetString(propItem.Value);
        }

        /// <summary>
        /// Gets the MIME type for an image.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> instance that this method extends.</param>
        /// <returns>The MIME type of the image or <c>null</c> if it cannot be determined.</returns>
        public static string GetMimeType(this Image image)
        {
            return ImageCodecInfo.GetImageDecoders().Where(id => id.FormatID == image.RawFormat.Guid).Select(id => id.MimeType).FirstOrDefault();
        }

        #region Private Methods
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    return encoders[i];
                }
            }
            return null;
        }
        #endregion
    }
}
