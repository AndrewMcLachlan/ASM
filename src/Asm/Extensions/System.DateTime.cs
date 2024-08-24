using System.Globalization;

namespace System;

/// <summary>
/// Extensions to the <see cref="DateTime"/> type.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Gets the first day of the week in the current culture.
    /// </summary>
    /// <param name="dt">The DateTime.</param>
    /// <returns>The first day of the given week.</returns>
    public static DateTime FirstDayOfWeek(this DateTime dt)
    {
        return FirstDayOfWeek(dt, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets the first day of the week.
    /// </summary>
    /// <param name="dt">The DateTime.</param>
    /// <param name="provider"> An System.IFormatProvider that supplies culture-specific formatting information.</param>
    /// <returns>The first day of the given week.</returns>
    public static DateTime FirstDayOfWeek(this DateTime dt, IFormatProvider provider)
    {
        DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(provider);
        DayOfWeek firstDay = info.FirstDayOfWeek;

        DateTime firstDayInWeek = dt.Date;

        while (firstDayInWeek.DayOfWeek != firstDay)
        {
            firstDayInWeek = firstDayInWeek.AddDays(-1);
        }

        return firstDayInWeek;
    }

    /// <summary>
    /// Gets the last day of the week in the current culture.
    /// </summary>
    /// <param name="dt">The DateTime.</param>
    /// <returns>The last day of the given week.</returns>
    public static DateTime LastDayOfWeek(this DateTime dt)
    {
        return LastDayOfWeek(dt, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets the last day of the week.
    /// </summary>
    /// <param name="dt">The DateTime.</param>
    /// <param name="provider"> An System.IFormatProvider that supplies culture-specific formatting information.</param>
    /// <returns>The last day of the given week.</returns>
    public static DateTime LastDayOfWeek(this DateTime dt, IFormatProvider provider)
    {
        DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(provider);
        DayOfWeek firstDay = info.FirstDayOfWeek;

        DateTime lastDayInWeek = dt.Date;

        while (lastDayInWeek.AddDays(1).DayOfWeek != firstDay)
        {
            lastDayInWeek = lastDayInWeek.AddDays(1);
        }

        return lastDayInWeek;
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="dt">The <see cref="DateTime"/> to convert.</param>
    /// <returns>The <see cref="DateOnly"/> equivalent.</returns>
    public static DateOnly ToDateOnly(this DateTime dt) => DateOnly.FromDateTime(dt);

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="dt">The <see cref="DateTime"/> to convert.</param>
    /// <returns>The <see cref="TimeOnly"/> equivalent.</returns>
    public static TimeOnly ToTimeOnly(this DateTime dt) => TimeOnly.FromDateTime(dt);

    /// <summary>
    /// Returns the number of months between two dates.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="other">The date to compare to.</param>
    /// <returns>The number of months between the dates.</returns>
    public static int DifferenceInMonths(this DateTime date, DateTime other)
    {
        DateTime earlier = date < other ? date : other;
        DateTime later = date < other ? other : date;

        var months = (later.Year - earlier.Year) * 12 + (later.Month - earlier.Month);

        if (months == 0) return months;

        // Round part months up or down
        return later.Day - earlier.Day < 0 ? months - 1 :
               later.Day - earlier.Day > 0 ? months + 1 :
               months;
    }

}
