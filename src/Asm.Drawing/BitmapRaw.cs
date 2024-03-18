using System.Drawing;
using System.Drawing.Imaging;
using Asm.Media;

namespace Asm.Drawing;

/// <summary>
/// A raw representation of a bitmap file.
/// </summary>
public class BitmapRaw
{
    #region Properties
    /// <summary>
    /// Gets or sets the file marker bits.
    /// </summary>
    public BitmapFileMarker FileMarker { get; set; }

    /// <summary>
    /// Gets or sets the header bits.
    /// </summary>
    public BitmapHeader BitmapHeader { get; set; }

    /// <summary>
    /// Gets or set the info header bits.
    /// </summary>
    public BitmapInfoHeader BitmapInfoHeader { get; set; }

    /// <summary>
    /// Gets or sets the pixel data.
    /// </summary>
    public byte[] PixelData { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Converts the bitmap to a 32 bit <see cref="Bitmap"/> type.
    /// </summary>
    /// <returns>A bitmap.</returns>
    public Bitmap ToBitmap32()
    {
        Bitmap bitmap = new(BitmapInfoHeader.Width, Math.Abs(BitmapInfoHeader.Height), GetPixelFormat(32));

        Color[] pixels = new Color[PixelData.Length / 4];
        for (int i = 0; i < PixelData.Length - 2; i += 4)
        {
            byte a = PixelData[i + 3];
            byte r = PixelData[i + 2];
            byte g = PixelData[i + 1];
            byte b = PixelData[i + 0];

            Color c = Color.FromArgb(a, r, g, b);
            pixels[i / 4] = c;
        }

        int w = 0;
        int h = (BitmapInfoHeader.Height < 1) ? 0 : BitmapInfoHeader.Height - 1;
        foreach (Color c in pixels)
        {
            bitmap.SetPixel(w, h, c);
            w++;

            if (w == BitmapInfoHeader.Width)
            {
                w = 0;
                if (BitmapInfoHeader.Height < 0)
                {
                    h++;
                }
                else
                {
                    h--;
                }
            }
        }

        return bitmap;
    }
    #endregion

    #region Private Methods
    private static PixelFormat GetPixelFormat(int colourDepth)
    {
        return colourDepth switch
        {
            32 => PixelFormat.Format32bppArgb,
            24 => PixelFormat.Format24bppRgb,
            16 => PixelFormat.Format16bppRgb555,
            8 => PixelFormat.Format8bppIndexed,
            4 => PixelFormat.Format4bppIndexed,
            _ => PixelFormat.DontCare,
        };
    }
    #endregion
}
