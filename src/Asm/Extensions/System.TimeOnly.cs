namespace System;

/// <summary>
/// Extension methods for <see cref="TimeOnly"/>.
/// </summary>
public static class TimeOnlyExtensions
{
    /// <summary>
    /// Static extensions for <see cref="TimeOnly"/>.
    /// </summary>
    extension(TimeOnly time)
    {
        /// <summary>
        /// Gets the current time.
        /// </summary>
        public static TimeOnly Now => TimeOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        public static TimeOnly UtcNow => TimeOnly.FromDateTime(DateTime.UtcNow);
    }
}
