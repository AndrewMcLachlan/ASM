using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// Extension methods for <see cref="IHtmlHelper"/>.
/// </summary>
public static class IHtmlHelperExtensions
{
    /// <summary>
    /// Determines whether the application is running in debug mode.
    /// </summary>
    /// <param name="html">The <see cref="IHtmlHelper"/> instance that this method extends.</param>
    /// <returns><see langword="true"/> if the application is running in debug mode; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDebug(this IHtmlHelper html) =>
#if DEBUG
        true;
#else
        false;
#endif
}
