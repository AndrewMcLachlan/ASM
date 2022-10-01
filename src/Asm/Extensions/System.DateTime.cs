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
}
