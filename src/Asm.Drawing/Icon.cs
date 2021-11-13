using System;
using System.Drawing;
using System.IO;
using Asm.Extensions;
using Asm.Media;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Asm.Drawing.Imaging;

namespace Asm.Drawing
{
    /// <summary>
    /// A class representing a Windows Icon.
    /// </summary>
    public class Icon : IDisposable
    {
        #region Fields
        private ImageFormat _imageFormat;
        #endregion

        #region Properties
        /// <summary>
        /// The image.
        /// </summary>
        public Image Image { get; protected set; }

        /// <summary>
        /// The icon's size in pixels.
        /// </summary>
        public Size Size
        {
            get { return new Size(Width, Height); }
        }

        /// <summary>
        /// The icon's height in pixels.
        /// </summary>
        public int Height
        {
            get { return FromByte(Header.Height); }
        }

        /// <summary>
        /// The icon's width in pixels.
        /// </summary>
        public int Width
        {
            get { return FromByte(Header.Width); }
        }

        /// <summary>
        /// The colour depth in bits.
        /// </summary>
        public int ColourDepth
        {
            get { return Header.Custom2 == 0 ? 32 : Header.Custom2; }
        }

        /// <summary>
        /// The colour planes.
        /// </summary>
        public int ColourPlanes
        {
            get { return Header.Custom1; }
        }

        /// <summary>
        /// The cursor's hotspot.
        /// </summary>
        public Point Hotspot
        {
            get { return new Point(Header.Custom1, Header.Custom2); }
        }

        /// <summary>
        /// The number of colours.
        /// </summary>
        public int Colours
        {
            get { return Header.Colours; }
        }

        /// <summary>
        /// Gets or sets the format of the icon's image.
        /// </summary>
        public ImageFormat Format
        {
            get
            {
                if (_imageFormat == null && Image != null) _imageFormat = Image.RawFormat;
                return _imageFormat;
            }
            set
            {
                _imageFormat = value;

                using (MemoryStream mem = new MemoryStream())
                {
                    Image.Save(mem, value);

                    mem.Position = 0;
                    Image = new Bitmap(mem);
                    RawImage = GetRawImage(Image);
                }
            }
        }

        internal IconDirEntry Header { get; private set; }

        internal byte[] RawImage { get; private set; }
        #endregion

        #region Constructors
        internal Icon(byte[] rawImage, IconDirEntry header)
        {
            RawImage = rawImage;
            Header = header;

            this.Image = GetImage(Header, RawImage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Icon"/> class.
        /// </summary>
        /// <param name="image">The icon image.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="image"/> is null.</exception>
        public Icon(System.Drawing.Image image)
        {
            if (image == null) throw new ArgumentNullException("image");

            Initialize(image);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Icon"/> class.
        /// </summary>
        /// <param name="imageData">A stream of image data.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="imageData"/> is null.</exception>
        public Icon(Stream imageData)
        {
            if (imageData == null) throw new ArgumentNullException("imageData");

            Image image = new Bitmap(imageData);

            Initialize(image);

            return;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Icon"/> class.
        /// </summary>
        /// <param name="icon">A Framework <see cref="System.Drawing.Icon"/> to convert.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="icon"/> is null.</exception>
        public Icon(System.Drawing.Icon icon)
        {
            if (icon == null) throw new ArgumentNullException("icon");

            using (MemoryStream mem = new MemoryStream())
            {
                icon.Save(mem);

                mem.Position = 0;
                IconParser parser = new IconParser(mem);

                Icon inner = (Icon)parser.Parse()[0];

                Header = inner.Header;
                Image = inner.Image;
                RawImage = inner.RawImage;
            }
        }

     /*   /// <summary>
        /// Initializes a new instance of the <see cref="Icon"/> class.
        /// </summary>
        /// <param name="cursor">A <see cref="System.Windows.Forms.Cursor"/> to convert.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cursor"/> is null.</exception>
        public Icon(System.Windows.Forms.Cursor cursor)
        {
            if (cursor == null) throw new ArgumentNullException("cursor");

            using (Image image = new Bitmap(cursor.Size.Width, cursor.Size.Height))
            {
                cursor.Draw(Graphics.FromImage(image), new Rectangle(0, 0, cursor.Size.Width, cursor.Size.Height));

                Image = image;
                RawImage = GetRawImage(image);

                Header = new IconDirEntry { Size = RawImage.Length, Offset = IconParser.SingleEntryCompleteHeaderSize, Custom1 = (short)cursor.HotSpot.X, Colours = Math.Min((byte)image.Palette.Entries.Length, (byte)255), Custom2 = (short)cursor.HotSpot.Y, Height = ToByte(cursor.Size.Height), Width = ToByte(cursor.Size.Width), Reserved = 0 };
            }
        }*/
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts this to an <see cref="System.Drawing.Icon"/>.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Icon ToIcon()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(BitConverter.GetBytes((short)IconParser.IconDirMarker), 0, 2);
                mem.Write(BitConverter.GetBytes((short)IconFileType.Icon), 0, 2);
                mem.Write(BitConverter.GetBytes((short)1), 0, 2);
                Header.WriteHeader(mem, IconParser.SingleEntryCompleteHeaderSize);

                mem.Write(RawImage, 0, RawImage.Length);
                mem.Position = 0;

                return new System.Drawing.Icon(mem);
            }
        }
        #endregion

        #region Internal Methods
        internal void Save(FileStream stream)
        {
            stream.Write(RawImage, 0, RawImage.Length);
        }
        #endregion

        #region Private Methods
        private void Initialize(Image image)
        {
            if (image == null) throw new ArgumentNullException("image");

            // If the image is larger than 256 then we must shrink it to size.
            if (image.Height > 256 || image.Width > 256)
            {
                int heightDivisor = image.Height / 256;
                int widthDivisor = image.Width / 256;

                int divisor = Math.Max(heightDivisor, widthDivisor);

                image = image.GetThumbnailImage(image.Height / divisor, image.Width / divisor, null, IntPtr.Zero);
            }

            RawImage = GetRawImage(image);

            Header = new IconDirEntry { Size = RawImage.Length, Offset = IconParser.SingleEntryCompleteHeaderSize, Custom2 = (short)image.GetColourDepth(), Colours = Math.Min((byte)image.Palette.Entries.Length, (byte)255), Custom1 = 0, Height = ToByte(image.Height), Width = ToByte(image.Width), Reserved = 0 };

            Image = image;
        }

        private static Image GetImage(IconDirEntry dirEntry, byte[] buffer)
        {
            ImageFormat format = ImageFormatHelper.GetImageFormat(buffer);

            if (format.Equals(ImageFormat.Png))
            {
                // IS PNG
                using (MemoryStream mem = new MemoryStream())
                {
                    mem.Write(buffer, 0, buffer.Length);
                    mem.Position = 0;
                    return new Bitmap(mem);
                }
            }
            else
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    mem.Write(BitConverter.GetBytes((short)IconParser.IconDirMarker), 0, 2);
                    mem.Write(BitConverter.GetBytes((short)IconFileType.Icon), 0, 2);
                    mem.Write(BitConverter.GetBytes((short)1), 0, 2);
                    dirEntry.WriteHeader(mem, IconParser.SingleEntryCompleteHeaderSize);

                    byte[] rawImage = buffer;
                    mem.Write(rawImage, 0, rawImage.Length);
                    mem.Position = 0;

                    return (new System.Drawing.Icon(mem)).ToBitmap();
                }

                /*int rawSize = Marshal.SizeOf(typeof(BitmapInfoHeader));

                BitmapInfoHeader infoHeader;

                IntPtr rawPtr = Marshal.AllocHGlobal(rawSize);
                try
                {
                    Marshal.Copy(buffer, 0, rawPtr, rawSize);
                    infoHeader = (BitmapInfoHeader)Marshal.PtrToStructure(rawPtr, typeof(BitmapInfoHeader));
                }
                finally
                {
                    Marshal.FreeHGlobal(rawPtr);
                }

                int xorMaskSize = XORMaskSize(infoHeader.ColourDepth, dirEntry.Height, dirEntry.Width);
                int andMaskSize = ANDMaskSize(infoHeader.ColourDepth, dirEntry.Height, dirEntry.Width);

                int pixelDataStart = infoHeader.Height > 0 ? rawSize : rawSize + andMaskSize;
                int andMaskStart = infoHeader.Height > 0 ? rawSize + xorMaskSize : rawSize;

                byte[] pixelData = new byte[xorMaskSize];
                Array.Copy(buffer, pixelDataStart, pixelData, 0, pixelData.Length);

                byte[] andMask = new byte[andMaskSize];
                Array.Copy(buffer, andMaskStart, andMask, 0, andMask.Length);

                byte[] pixelData32;

                if (infoHeader.ColourDepth < 32)
                {
                    // Add extra byte per pixel for alpha channel
                    pixelData32 = new byte[pixelData.Length + pixelData.Length / 3];

                    int k = 0;
                    for (int i = 0; i < pixelData.Length; i += 3)
                    {
                        pixelData32[k] = pixelData[i];
                        pixelData32[k + 1] = pixelData[i + 1];
                        pixelData32[k + 2] = pixelData[i + 2];
                        pixelData32[k + 3] = 255;
                        k += 4;
                    }

                    // Apply AND mask 
                    int bitsPerLine = dirEntry.Width;
                    int bytesPerLine = BytesPerLine(infoHeader.ColourDepth, dirEntry.Width);
                    int insignificantBitsPerLine = (bytesPerLine * 8) - bitsPerLine;
                    int insignificantBytesPerLine = insignificantBitsPerLine / 8;

                    int counter = 0;
                    int pixelCounter = 3;
                    for (int j = 0; j < dirEntry.Height; j++)
                    {
                        for (int n = 0; n < dirEntry.Width; n+=8)
                        {
                            byte b = andMask[counter];
                            for (int i = 0; i < 8 && n + i < dirEntry.Width; i++)
                            {
                                int shift = b >> (7-i);
                                if ((shift & 1) == 1)
                                {
                                    if (pixelData32[pixelCounter - 3] > 0 || pixelData32[pixelCounter - 2] > 0 || pixelData32[pixelCounter - 1] > 0)
                                    {
                                        throw new Exception();
                                    }

                                    pixelData32[pixelCounter] = 0;
                                }
                                pixelCounter+=4;
                            }

                            counter++;
                            if (n + 8 >= dirEntry.Width)
                            {
                                counter += insignificantBytesPerLine;
                            }
                        }
                    }
                }
                else
                {
                    pixelData32 = pixelData;
                }

                // If necessary, reverse the pixel data so the image appears the right way round.
                byte[] pixelData32Reversed = new byte[pixelData32.Length];

                if (infoHeader.Height > 0)
                {
                    int fullWidth = dirEntry.Width * 4;
                    int l = (dirEntry.Height - 1) * fullWidth;
                    for (int i = 0; i < dirEntry.Height * fullWidth; i += fullWidth)
                    {
                        for (int j = 0; j < fullWidth; j++)
                        {
                            pixelData32Reversed[l + j] = pixelData32[i + j];
                        }
                        l -= fullWidth;
                    }
                }
                else
                {
                    pixelData32Reversed = pixelData32;
                }
                PixelFormat format = GetPixelFormat(dirEntry);
                format = PixelFormat.Format32bppArgb;

                // Create new bitmap
                Bitmap bm = new Bitmap(dirEntry.Height, dirEntry.Width, format);

                BitmapData data = bm.LockBits(new Rectangle(0, 0, dirEntry.Width, dirEntry.Height), ImageLockMode.WriteOnly, format);
                Marshal.Copy(pixelData32Reversed, 0, data.Scan0, pixelData32Reversed.Length);
                bm.UnlockBits(data);

                return bm;*/
            }

            throw new InvalidOperationException();
        }

        private static PixelFormat GetPixelFormat(IconDirEntry dirEntry)
        {
            switch (dirEntry.Custom2)
            {
                case 32:
                    return PixelFormat.Format32bppArgb;
                case 24:
                    return PixelFormat.Format24bppRgb;
                case 16:
                    return PixelFormat.Format16bppRgb555;
                case 8:
                    return PixelFormat.Format8bppIndexed;
                case 4:
                    return PixelFormat.Format4bppIndexed;
                default:
                    return PixelFormat.DontCare;
            }


        }

        private static byte[] GetRawImage(System.Drawing.Image image)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                if (image.RawFormat.Equals(ImageFormat.Png))
                {
                    image.Save(mem, System.Drawing.Imaging.ImageFormat.Png);

                    mem.Position = 0;

                    byte[] buffer = new byte[mem.Length];

                    mem.Read(buffer, 0, buffer.Length);

                    return buffer;
                }
                else
                {
                    using (Bitmap bitmap = new Bitmap(image))
                    {
                        IntPtr iconHandle = bitmap.GetHicon();
                        System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(iconHandle);

                        icon.Save(mem);
                        mem.Position = 0;
                        IconParser ip = new IconParser(mem);
                        return ip.Parse()[0].RawImage;
                    }

                    /*throw new NotImplementedException();
                    //image.Save(mem, System.Drawing.Imaging.ImageFormat.Bmp);
                    Bitmap bitmap = new Bitmap(image);

                    BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    bitmap.UnlockBits(data);


                    byte[] bmp = new byte[mem.Length];
                    mem.Read(bmp, 0, mem.Length);

                    //return StripBmpHeader(bmp);*/
                }
            }
        }

        /*private byte[] StripBmpHeader(byte[] bmp)
        {
            const int BitMapHeaderSize = BitmapHeader.HeaderSize;

            int resultSize = bmp.Length - BitMapHeaderSize;

            byte[] result = new byte[resultSize];

            Array.Copy(bmp, BitMapHeaderSize, result, 0, resultSize);

            return result;
        }*/

        private static byte ToByte(int dimension)
        {
            if (dimension > 256) throw new ArgumentException("Dimension is too large", "dimension");

            return dimension == 256 ? (byte)0 : checked((byte)dimension);
        }

        private static int FromByte(byte dimension)
        {
            return dimension == 0 ? 256 : dimension;
        }

        private static int XORMaskSize(int colourDepth, int height, int width)
        {
            int bytesPerPixel = colourDepth / 8;
            //int bytesPerPixel = 4;

            return width * height * bytesPerPixel;
        }
        #endregion

        #region Dispose Pattern
        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~Icon()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether or not this object is in the process of being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Image.Dispose();
                }
            }

            _disposed = true;
        }
        #endregion

        #region Equality Operators
        /// <summary>
        /// Compares one icon to another.
        /// </summary>
        /// <param name="obj">The icon to compare.</param>
        /// <returns>true if the icons match, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is Icon)
            {
                return CheckEquality(this, (Icon)obj);
            }
            else
            {
                // Attempt a string comparison.
                return this.ToString().Equals(obj.ToString(), StringComparison.CurrentCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Compares one icon to another.
        /// </summary>
        /// <param name="left">The left-hand icon.</param>
        /// <param name="right">The right-hand icon.</param>
        /// <returns>true if the icons match, otherwise false.</returns>
        public static bool operator ==(Icon left, Icon right)
        {
            return CheckEquality(left, right);
        }

        /// <summary>
        /// Compares one icon to another.
        /// </summary>
        /// <param name="left">The left-hand icon.</param>
        /// <param name="right">The right-hand icon.</param>
        /// <returns>true if the icons do not match, otherwise false.</returns>
        public static bool operator !=(Icon left, Icon right)
        {
            return !CheckEquality(left, right);
        }

        /// <summary>
        /// Gets the hash code for this icon.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Image.GetHashCode() ^ RawImage.GetHashCode();
        }

        private static bool CheckEquality(Icon left, Icon right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            // At this stage we know that at least one of these is not null. If the other is, return false.
            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.Image.Equals(right.Image) && left.Header.Equals(right.Header) && left.RawImage.Equals(right.RawImage);
        }
        #endregion
    }
}
