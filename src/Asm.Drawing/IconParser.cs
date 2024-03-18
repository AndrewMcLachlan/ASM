using System.Runtime.InteropServices;

namespace Asm.Drawing;

/// <summary>
/// IconParser.
/// </summary>
internal sealed class IconParser : IDisposable
{
    #region Types
    private delegate void ParseDelegate();
    #endregion

    #region Constants
    internal const short IconDirMarker = 0;
    internal const int HeaderSize = 6;
    internal const int DirEntrySize = 16;
    internal const int SingleEntryCompleteHeaderSize = 22;
    #endregion

    #region Fields
    private bool completed;
    private ParseDelegate parser;
    private IAsyncResult _result;
    private List<Icon> set = new List<Icon>();
    #endregion

    #region Properties
    internal Stream BaseStream { get; private set; }
    internal IconFileType FileType { get; private set; }
    #endregion

    #region Constructors
    internal IconParser()
    {
    }

    internal IconParser(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException("stream");
        BaseStream = stream;
    }
    #endregion

    #region Internal Methods
    internal List<Icon> Parse()
    {
        DoParse();
        return set;
    }

    internal IAsyncResult BeginParse()
    {
        parser = new ParseDelegate(DoParse);
        _result = parser.BeginInvoke(Complete, null);
        return _result;
    }

    internal List<Icon> EndParse()
    {
        if (!completed)
        {
            parser.EndInvoke(_result);
        }
        return set;
    }
    #endregion

    #region Private Methods
    private void DoParse()
    {
        List<IconDirEntry> rawContents = new List<IconDirEntry>();

        byte[] buffer;

        #region Read Header
        IconHeader header;

        int headerSize = Marshal.SizeOf(typeof(IconHeader));

        IntPtr headerPtr = Marshal.AllocHGlobal(headerSize);

        try
        {
            buffer = new byte[headerSize];
            BaseStream.Read(buffer, 0, headerSize);
            Marshal.Copy(buffer, 0, headerPtr, headerSize);
            header = (IconHeader)Marshal.PtrToStructure(headerPtr, typeof(IconHeader));
        }
        finally
        {
            Marshal.FreeHGlobal(headerPtr);
        }

        // Validation
        if (header.FileMarker != IconDirMarker)
        {
            //TODO: Error handling
            return;
        }

        try
        {
            FileType = (IconFileType)header.FileType;
        }
        catch (InvalidCastException)
        {
            //TODO: Error handling
            return;
        }
        #endregion

        for (int i = 0; i < header.NumberOfImages; i++)
        {
            IconDirEntry raw;

            int dirSize = Marshal.SizeOf(typeof(IconDirEntry));

            IntPtr dirPtr = Marshal.AllocHGlobal(dirSize);
            try
            {
                buffer = new byte[dirSize];
                BaseStream.Read(buffer, 0, dirSize);
                Marshal.Copy(buffer, 0, dirPtr, dirSize);
                raw = (IconDirEntry)Marshal.PtrToStructure(dirPtr, typeof(IconDirEntry));
            }
            finally
            {
                Marshal.FreeHGlobal(dirPtr);
            }

            rawContents.Add(raw);
        }

        foreach (IconDirEntry raw in rawContents)
        {
            buffer = new byte[raw.Size];

            // If the stream is not seekable we must assume that the icon data is in the same order as the directory
            if (BaseStream.CanSeek)
            {
                BaseStream.Seek(raw.Offset, SeekOrigin.Begin);
            }

            BaseStream.Read(buffer, 0, raw.Size);

            set.Add(new Icon(buffer, raw));
        }

        /*foreach (RawIconFile raw in rawContents)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(BitConverter.GetBytes((short)IconDirMarker), 0, 2);
                mem.Write(BitConverter.GetBytes((short)FileType), 0, 2);
                mem.Write(BitConverter.GetBytes((short)1), 0, 2);
                raw.WriteHeader(mem, SingleEntryCompleteHeaderSize);
                mem.Write(raw.ImageData, 0, raw.ImageData.Length);
                mem.Position = 0;

                if (raw is RawIcon)
                {
                    // TODO: Create image from image data
                    //set.Add(new Icon(icon, (RawIcon)raw));
                }
            }
        }*/
    }

    private void Complete(IAsyncResult result)
    {
        completed = true;
    }
    #endregion

    #region Dispose Pattern
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~IconParser()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BaseStream.Dispose();
            }

            BaseStream = null;
        }

        _disposed = true;
    }
    #endregion
}
