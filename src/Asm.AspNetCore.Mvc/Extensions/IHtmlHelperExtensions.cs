using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Mvc.Rendering;

public static class IHtmlHelperExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDebug(this IHtmlHelper html) =>
#if DEBUG
        true;
#else
        false;
#endif
}
