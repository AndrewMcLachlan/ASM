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

        /// <summary>
        /// Converts the string to its most sensible casing.
        /// </summary>
        /// <param name="input">The string.</param>
        /// <returns>A sensibly cased string.</returns>
        public static string ToSensibleCase(this string input)
        {
            if (input == null)
            {
                return null;
            }
            /*Regex upperCase = new Regex("^[A-Z]+$");
            Regex lowerCase = new Regex("^[a-z]+$");

            string result = input;

            if (String.IsNullOrEmpty(input))
            {
            }
            else if (upperCase.IsMatch(input) || lowerCase.IsMatch(input))
            {
                result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());
            }
            else if (lowerCase.IsMatch(input.Substring(0, 1)))
            {
                result = input.Substring(0, 1).ToUpperInvariant() + input.Substring(1);
            }

            return result;*/

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToUpperInvariant());
        }
    }
}
