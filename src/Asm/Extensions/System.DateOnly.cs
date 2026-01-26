namespace System;

/// <summary>
/// Extension methods for <see cref="DateOnly"/>.
/// </summary>
public static class DateOnlyExtensions
{
    /// <summary>
    /// Static extensions for <see cref="DateOnly"/>.
    /// </summary>
    extension(DateOnly date)
    {
        /// <summary>
        /// Gets the current date.
        /// </summary>
        public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

        /// <summary>
        /// Converts a <see cref="DateOnly"/> to a <see cref="DateTime"/> at the start of the day.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> instance with the time component set to the start of the day.</returns>
        public DateTime ToStartOfDay() => date.ToDateTime(TimeOnly.MinValue);

        /// <summary>
        /// Converts a <see cref="DateOnly"/> to a <see cref="DateTime"/> at the end of the day.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> instance with the time component set to the end of the day.</returns>
        public DateTime ToEndOfDay() => date.ToDateTime(TimeOnly.MaxValue);

        /// <summary>
        /// Returns the number of months between two dates.
        /// </summary>
        /// <param name="other">The date to compare to.</param>
        /// <returns>The number of months between the dates.</returns>
        public int DifferenceInMonths(DateOnly other)
        {
            DateOnly earlier = date < other ? date : other;
            DateOnly later = date < other ? other : date;

            var months = (later.Year - earlier.Year) * 12 + (later.Month - earlier.Month);

            if (months == 0) return months;

            // Round part months up or down
            return later.Day - earlier.Day < 0 ? months - 1 :
                   later.Day - earlier.Day > 0 ? months + 1 :
                   months;
        }
    }
}
