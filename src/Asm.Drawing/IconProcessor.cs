using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Asm.Extensions;

namespace Asm.Drawing
{
    internal static class IconProcessor
    {
        public static void ApplyAndMask(byte[] andMask, ref byte[] pixelData, int height, int width, int colourDepth)
        {
            int paddingBytes = PaddingBytesPerLine(1, width);

            int counter = 0;
            int pixelCounter = 3;
            for (int j = 0; j < height; j++)
            {
                for (int n = 0; n < width; n += 8)
                {
                    byte b = andMask[counter];
                    for (int i = 0; i < 8 && n + i < width; i++)
                    {
                        int shift = b >> (7 - i);
                        if ((shift & 1) == 1)
                        {
                            if (pixelData[pixelCounter - 3] > 0 || pixelData[pixelCounter - 2] > 0 || pixelData[pixelCounter - 1] > 0)
                            {
                                // TODO: Expand exception info.
                                throw new InvalidOperationException();
                            }

                            pixelData[pixelCounter] = 0;
                        }
                        pixelCounter += 4;
                    }

                    counter++;
                    if (n + 8 >= width)
                    {
                        counter += paddingBytes;
                    }
                }
            }
        }

        public static int PaddingBytesPerLine(int colourDepth, int width)
        {
            int bitsPerLine = colourDepth * width;
            int bytesPerLine = BytesPerLine(colourDepth, width);
            int insignificantBitsPerLine = (bytesPerLine * 8) - bitsPerLine;
            int insignificantBytesPerLine = insignificantBitsPerLine / 8;

            return insignificantBytesPerLine;
        }

        public static byte[] CreateAndMask(Image image)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                byte[] andMask = new byte[BitmapSize(1, image.Height, image.Width)];

                int counter = 0;

                int bitsPerLine = image.Width;
                int bytesPerLine = BytesPerLine(image.GetColourDepth(), image.Width);
                int insignificantBitsPerLine = (bytesPerLine * 8) - bitsPerLine;
                int insignificantBytesPerLine = insignificantBitsPerLine / 8;

                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j += 8)
                    {
                        byte current = 0;
                        for (int k = 0; k < 8 && counter < image.Width; k++)
                        {
                            Color color = bitmap.GetPixel(j + k, i);

                            if (color.A == Byte.MaxValue)
                            {
                                current = checked((byte)(current | 1));
                                current = checked((byte)(current << 1));
                            }
                        }

                        andMask[counter] = current;

                        counter++;
                        if (j + 8 >= image.Width)
                        {
                            counter += insignificantBytesPerLine;
                        }
                    }
                }

                return andMask;
            }
        }

        public static int BitmapSize(int colourDepth, int height, int width)
        {
            //int and_line_length = (width + 7) >> 3;
            //and_line_length = (and_line_length + 3) & ~3;
            //int and_mask_size = and_line_length * height;

            int bytesPerLine = BytesPerLine(colourDepth, width);

            //return and_mask_size;

            return bytesPerLine * height;
        }

        public static int BytesPerLine(int colourDepth, int width)
        {
            //int and_line_length = (width + 7) >> 3;
            //return (and_line_length + 3) & ~3;

            double bytesPerPixel = (double)colourDepth / 8d;

            int bytesPerLine = (int)Math.Ceiling(width * bytesPerPixel);

            int pad = 4 - (bytesPerLine % 4) == 4 ? 0 : 4 - (bytesPerLine % 4);
            bytesPerLine += pad;

            return bytesPerLine;
        }
    }
}
