using System;

namespace Asm.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="Decimal"/> type.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Coverts to a rounded currency representation.
        /// </summary>
        /// <param name="value">The decimal.</param>
        /// <returns>A string rounded away from zero to two decimal places.</returns>
        public static string ToRoundedCurrencyString(this Decimal value)
        {
            return value.ToRoundedCurrencyString(2);
        }

        /// <summary>
        /// Coverts to a rounded currency representation.
        /// </summary>
        /// <param name="value">The decimal.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A string rounded away from zero.</returns>
        public static string ToRoundedCurrencyString(this Decimal value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero).ToString("N" + decimalPlaces);
        }
    }
}