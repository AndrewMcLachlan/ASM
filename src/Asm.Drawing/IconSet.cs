using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Asm.Drawing
{
    /// <summary>
    /// A collection of icons in a single file.
    /// </summary>
    public class IconSet : IDisposable
    {
        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the icon file's name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the icons in the file.
        /// </summary>
        public IList<Icon> Icons { get; private set; }

        /// <summary>
        /// Gets the type of the icon file.
        /// </summary>
        public IconFileType FileType { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IconSet" /> class.
        /// </summary>
        public IconSet(IconFileType type)
        {
            Icons = new List<Icon>();

            FileType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IconSet" /> class.
        /// </summary>
        /// <param name="stream">A stream containing icon file data.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        public IconSet(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            Parse(stream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IconSet" /> class.
        /// </summary>
        /// <param name="fileName">The name of the icon file to load.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fileName"/> is null.</exception>
        public IconSet(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            FileName = fileName;

            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Parse(stream);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Saves the icon file to disk.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the <see cref="FileName"/> property is not set.</exception>
        public void Save()
        {
            if (String.IsNullOrWhiteSpace(FileName)) throw new InvalidOperationException("FileName not set");

            using (FileStream stream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                int offset = IconParser.HeaderSize + (IconParser.DirEntrySize * Icons.Count);

                IconHeader header = new IconHeader { FileType = (short)FileType, NumberOfImages = (short)Icons.Count };
                header.Write(stream);

                foreach (Icon icon in Icons)
                {
                    icon.Header.WriteHeader(stream, offset);
                    offset += icon.Header.Size;
                }

                foreach (Icon icon in Icons)
                {
                    icon.Save(stream);
                }
            }
        }
        #endregion

        #region Private Methods
        private void Parse(Stream stream)
        {
            IconParser parser = new IconParser(stream);
            Icons = parser.Parse();
            FileType = parser.FileType;
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether or not this object is in the process of being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (Icon icon in Icons)
                    {
                        icon.Dispose();
                    }
                    Icons.Clear();
                }

                // Dispose unmanaged resources.
            }

            _disposed = true;
        }
        #endregion
    }
}
