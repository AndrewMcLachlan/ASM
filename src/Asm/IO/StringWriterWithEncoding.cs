using System.Text;

namespace Asm.IO;

/// <summary>
/// A <see cref="System.IO.StringWriter" /> implementation that allows custom encodings.
/// </summary>
public sealed class StringWriterWithEncoding : StringWriter
{
    private readonly Encoding _encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class.
    /// </summary>
    /// <param name="encoding"> The Encoding in which the output is written.</param>
    public StringWriterWithEncoding(Encoding encoding)
    {
        this._encoding = encoding;
    }

    /// <summary>
    /// Gets the System.Text.Encoding in which the output is written.
    /// </summary>
    /// <returns>
    ///  The Encoding in which the output is written.
    /// </returns>
    public override Encoding Encoding
    {
        get { return _encoding; }
    }
}
