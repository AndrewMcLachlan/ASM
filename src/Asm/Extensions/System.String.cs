using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Asm.Extensions
{
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

        public static string Squish(this string str, int fromStart, int fromEnd)
        {
            return str.Substring(fromStart, str.Length - (fromStart+fromEnd));
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
            if (input == null)
            {
                return null;
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToUpperInvariant());
        }
    }
}
