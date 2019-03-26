using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asm.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Boolean"/> class.
    /// </summary>
    public static class BooleanExtensions
    {
        /// <summary>
        /// Converts this boolean to a numeric value.
        /// </summary>
        /// <param name="b">The boolean.</param>
        /// <returns>1 if the value is true, otherwise 0.</returns>
        public static int ToNumeric(this bool b)
        {
            return b ? 1 : 0;
        }

        /// <summary>
        /// Converts this boolean to a string representation of it's numeric value.
        /// </summary>
        /// <param name="b">The boolean.</param>
        /// <returns>"1" if the value is true, otherwise "0".</returns>
        public static string ToNumericString(this bool b)
        {
            return b.ToNumeric().ToString();
        }
    }
}
