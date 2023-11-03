using System.Globalization;
using System.Text.RegularExpressions;

namespace System;

/// <summary>
/// Extensions for the <see cref="System.String"/> class.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Appends a string to a string, using the separator string.
    /// </summary>
    /// <remarks>
    /// The separator string is not used if the string is empty.
    /// </remarks>
    /// <param name="str">The string to append to.</param>
    /// <param name="value">The string to append.</param>
    /// <param name="separator">The string that separates the two strings.</param>
    /// <returns>The appended string.</returns>
    public static string Append(this string str, string value, string separator)
    {
        if (str.Length == 0)
        {
            return str + value;
        }
        else
        {
            return str + separator + value;
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
    /// <param name="str">The string to prepend to.</param>
    /// <param name="value">The string to prepend.</param>
    /// <param name="separator">The string that separates the two strings.</param>
    /// <returns>The prepended string.</returns>
    public static string Prepend(this string str, string value, string separator)
    {
        if (str.Length == 0)
        {
            return value + str;
        }
        else
        {
            return value + separator + str;
        }
    }

    /// <summary>
    /// Converts the string to title case.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>A title cased string.</returns>
    public static string ToTitleCase(this string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToUpperInvariant());
    }


    #region ToMachine
    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex SpecialCharactersRegex();
    private static readonly Regex SpecialCharacters = SpecialCharactersRegex();

    [GeneratedRegex("--+")]
    private static partial Regex MultipleDashRegex();
    private static readonly Regex MultipleDash = MultipleDashRegex();

    /// <summary>
    /// Converts the string to a machine-readable name, lower case with spaces and special characters replaced with hyphens.
    /// </summary>
    /// <example>
    /// "Fruit &amp; Veg" becomes "fruit-veg".
    /// </example>
    /// <param name="str">The string instance that this method extends.</param>
    /// <returns>A machine-readable version of the string.</returns>
    public static string ToMachine(this string str) =>
        MultipleDash.Replace(
            SpecialCharacters.Replace(str.ToLowerInvariant(), "-"),
            "-"
        );
    #endregion
}
