using System.Globalization;

namespace System;

/// <summary>
/// Extensions for the <see cref="System.String"/> class.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Appends a string to a string, using the separator string.
    /// </summary>
    /// <remarks>
    /// The separator string is not used if the string is empty.
    /// </remarks>
    /// <param name="source">The string to append to.</param>
    /// <param name="value">The string to append.</param>
    /// <param name="separator">The string that separates the two strings.</param>
    /// <returns>The appended string.</returns>
    public static string Append(this string source, string value, string separator)
    {
        if (source.Length == 0)
        {
            return source + value;
        }
        else
        {
            return source + separator + value;
        }
    }

    /// <summary>
    /// Reduces a string in length from beginning and/or end.
    /// </summary>
    /// <param name="str">The string to squish.</param>
    /// <param name="fromStart">The number of characters to remove from the beginning of the string.</param>
    /// <param name="fromEnd">The number of character to remove from the end of the string.</param>
    /// <returns>A new string that has been squished.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="fromStart"/> is less than zero or greater than the length of th string.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="fromEnd"/> is less than zero or greater than the length of th string.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the total number of characters to be removed exceeds the length of the string.</exception>
    public static string Squish(this string str, int? fromStart = null, int? fromEnd = null)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        if (fromStart < 0 || fromStart > str.Length) throw new ArgumentOutOfRangeException(nameof(fromStart), $"{nameof(fromStart)} must be greater than zero and less than the length of the string");
        if (fromEnd < 0 || fromEnd > str.Length) throw new ArgumentOutOfRangeException(nameof(fromStart), $"{nameof(fromStart)} must be greater than zero and less than the length of the string");
        if (fromStart + fromEnd > str.Length) throw new InvalidOperationException("The total number of characters to be removed exceeds the length of the string");

        return str.Substring(fromStart ?? 0, str.Length - (fromStart+fromEnd ?? 0));
    }

    /// <summary>
    /// Prepends a string to a string, using the separator string.
    /// </summary>
    /// <remarks>
    /// The separator string is not used if the string is empty.
    /// </remarks>
    /// <param name="source">The string to prepend to.</param>
    /// <param name="value">The string to prepend.</param>
    /// <param name="separator">The string that separates the two strings.</param>
    /// <returns>The prepended string.</returns>
    public static string Prepend(this string source, string value, string separator)
    {
        if (source.Length == 0)
        {
            return value + source;
        }
        else
        {
            return value + separator + source;
        }
    }

    /// <summary>
    /// Converts the string to title case.
    /// </summary>
    /// <param name="input">The string.</param>
    /// <returns>A title cased string.</returns>
    public static string ToTitleCase(this string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToUpperInvariant());
    }
}
