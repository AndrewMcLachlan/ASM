namespace Asm.Testing;

/// <summary>
/// Extensions for the <see cref="String" /> class.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Replaces backslash-encoded whitespace characters with their actual values.
    /// </summary>
    /// <remarks>
    /// Supports '\r', '\n', and '\t'.
    /// </remarks>
    /// <param name="str">The string to decode.</param>
    /// <returns>The decoded string.</returns>
    public static string? DecodeWhitespace(this string str)
    {
        if (str == null) return null;

        return str.Replace(@"\r", "\r").Replace(@"\n", "\n").Replace(@"\t", "\t");
    }
}
