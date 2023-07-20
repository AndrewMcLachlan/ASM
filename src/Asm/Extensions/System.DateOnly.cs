namespace System;

public static class DateOnlyExtensions
{
    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.Today);

    public static DateTime ToStartOfDay(this DateOnly date) => date.ToDateTime(TimeOnly.MinValue);

    public static DateTime ToEndOfDay(this DateOnly date) => date.ToDateTime(TimeOnly.MaxValue);

    public static int DifferenceInMonths(this DateOnly date, DateOnly other)
    {
        var months = Math.Abs((date.Year - other.Year) * 12 + (date.Month - other.Month));

        // Round part months up or down
        return date.Day - other.Day < 0 ? months + 1 :
               date.Day - other.Day > 0 ? months - 1 :
               months;
    }
}
