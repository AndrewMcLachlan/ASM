using System.Text;

namespace Asm.IO;

/// <summary>
/// A <see cref="System.IO.StringWriter" /> implementation that allows custom encodings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class.
/// </remarks>
/// <param name="encoding"> The Encoding in which the output is written.</param>
public sealed class StringWriterWithEncoding(Encoding encoding) : StringWriter
{
    /// <summary>
    /// Gets the System.Text.Encoding in which the output is written.
    /// </summary>
    /// <returns>
    ///  The Encoding in which the output is written.
    /// </returns>
    public override Encoding Encoding
    {
        get { return encoding; }
    }
}
