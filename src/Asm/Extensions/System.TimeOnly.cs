namespace Asm.Extensions;

/// <summary>
/// Extension methods for <see cref="TimeOnly"/>.
/// </summary>
public static class TimeOnlyExtensions
{
    /// <summary>
    /// Static extensions for <see cref="TimeOnly"/>.
    /// </summary>
    extension(TimeOnly)
    {
        /// <summary>
        /// Gets the current date.
        /// </summary>
        public static TimeOnly Now => TimeOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Gets the current date.
        /// </summary>
        public static TimeOnly UtcNow => TimeOnly.FromDateTime(DateTime.UtcNow);
    }
}
