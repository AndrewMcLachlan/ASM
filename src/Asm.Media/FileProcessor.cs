using System;

namespace Asm.Media;

/// <summary>
/// Takes a movie file, determines its type and retrieves header information.
/// </summary>
public sealed class FileProcessor
{
    private FileProcessor()
    {
    }

    /// <summary>
    /// Retrieves meta data from a movie file.
    /// </summary>
    /// <param name="fileName">The name and path of the file to process.</param>
    /// <returns>A <see cref="Media"/> object containing information about the file. If the file is not recognised, an empty instance of <see cref="Media"/> is returned.</returns>
    /// <remarks>
    /// This currently handles MPEG, Windows Media, Quicktime and Real Media files.
    /// Supported Extensions: M2P, M2T, M2V, MP2, MP2V, MPE, MPEG, MPG, MPV2, MOV, QT, RM, RAM, ASF, WMV.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fileName"/> is null.</exception>
    public static MediaFile ProcessFile(string fileName)
    {
        if (fileName == null) throw new ArgumentNullException("fileName");
        string extension;

        extension = fileName.Substring(fileName.LastIndexOf(".") + 1);

        switch (extension.ToUpper())
        {
            case "M2P":
            case "M2T":
            case "M2V":
            case "MP2":
            case "MP2V":
            case "MPE":
            case "MPEG":
            case "MPG":
            case "MPV2":
                Mpeg mpeg = new Mpeg(fileName);
                mpeg.Read();
                return mpeg;
            case "MOV":
            case "QT":
                Quicktime qt = new Quicktime(fileName);
                qt.Read();
                return qt;
            case "RM":
            case "RAM":
                RealMedia rm = new RealMedia(fileName);
                rm.Read();
                return rm;
            case "ASF":
            case "WMV":
                WindowsMedia wm = new WindowsMedia(fileName);
                wm.Read();
                return wm;
            default:
                throw new NotSupportedException();
        }
    }
}
