namespace System;

public static class DateOnlyExtensions
{
    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.Today);

    public static DateTime ToStartOfDay(this DateOnly date) => date.ToDateTime(TimeOnly.MinValue);

    public static DateTime ToEndOfDay(this DateOnly date) => date.ToDateTime(TimeOnly.MaxValue);
}
