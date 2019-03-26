using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asm.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="StringBuilder"/> class.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the string returned by processing a composite format string, which 
        /// contains zero or more format items, along with the default line terminator, to this instance. Each format item is 
        /// replaced by the string representation of a corresponding argument in a parameter 
        /// array using a specified format provider.
        /// </summary>
        /// <param name="source">The StringBuilder.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <returns>
        /// A reference to this instance after the append operation has completed. After
        /// the append operation, this instance contains any data that existed before
        /// the operation, suffixed by a copy of format where any format specification
        /// is replaced by the string representation of the corresponding object argument.
        /// </returns>
        public static StringBuilder AppendFormatLine(this StringBuilder source, IFormatProvider provider, string format, params object[] args)
        {
            source.AppendFormat(provider, format, args);
            return source.AppendLine();
        }
    }
}
