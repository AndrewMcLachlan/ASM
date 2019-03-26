using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asm
{
    /// <summary>
    /// Converts units of measurement.
    /// </summary>
    public static class UnitConvert
    {
        #region Length
        /// <summary>
        /// The number of inches in a foor.
        /// </summary>
        public const int InchesPerFoot = 12;
        
        /// <summary>
        /// The number of metres in an inch.
        /// </summary>
        public const double MetresPerInch = 0.0254;

        /// <summary>
        /// The number of inches in a metre.
        /// </summary>
        public const double InchesPerMetre = 1 / MetresPerInch;

        /// <summary>
        /// Converts inches to metres.
        /// </summary>
        /// <param name="inches">Inches.</param>
        /// <returns><paramref name="inches"/> converted to metres.</returns>
        public static double InchesToM(double inches)
        {
            return inches * MetresPerInch;
        }

        /// <summary>
        /// Converts feet and inches to metres.
        /// </summary>
        /// <remarks>
        /// There is nothing to stop <paramref name="inches"/> being larger than 12, the conversion will still be accurate.
        /// </remarks>
        /// <param name="feet">Feet.</param>
        /// <param name="inches">Inches.</param>
        /// <returns><paramref name="feet"/> and <paramref name="inches"/> in metres.</returns>
        public static double FeetToM(double feet, double inches)
        {
            return InchesToM((feet * InchesPerFoot) + inches);
        }
        #endregion

        #region Weights
        /// <summary>
        /// The number of ounces in a pound.
        /// </summary>
        public const int OuncesPerPound = 16;

        /// <summary>
        /// The number of pounds in a stone.
        /// </summary>
        public const int PoundsPerStone = 14;

        /// <summary>
        /// The number of kilograms in a pound.
        /// </summary>
        public const double KilogramsPerPound = 0.4539;

        /// <summary>
        /// The number of pounds in a kilogram.
        /// </summary>
        public const double PoundsPerKilogram = 1 / KilogramsPerPound;

        /// <summary>
        /// Converts pounds to kilograms.
        /// </summary>
        /// <param name="pounds">Pounds.</param>
        /// <returns><paramref name="pounds"/> converted to kilograms.</returns>
        public static double PoundsToKg(double pounds)
        {
            return pounds * KilogramsPerPound;
        }

        /// <summary>
        /// Converts stones and pounds to kilograms.
        /// </summary>
        /// <remarks>
        /// There is nothing to stop <paramref name="pounds"/> being larger than 14, the conversion will still be accurate.
        /// </remarks>
        /// <param name="stones">Stones.</param>
        /// <param name="pounds">Pounds.</param>
        /// <returns><paramref name="stones"/> and <paramref name="pounds"/> in kilograms.</returns>
        public static double StonesToKg(double stones, double pounds)
        {
            return PoundsToKg((stones * PoundsPerStone) + pounds);
        }
        #endregion
    }
}
