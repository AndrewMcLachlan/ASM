namespace System.Collections;

/// <summary>
/// Extension methods for the <see cref="System.Collections.IEnumerator"/> interface.
/// </summary>
public static class AsmEnumeratorExtensions
{
    /// <summary>
    /// Converts an <see cref="System.Collections.IEnumerator"/> instance to an <see cref="System.Collections.Generic.IEnumerator{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the generic enumerator.</typeparam>
    /// <param name="iEnumerator">The <see cref="System.Collections.IEnumerator"/>.</param>
    /// <returns>An instance of <see cref="System.Collections.Generic.IEnumerator{T}"/>.</returns>
    public static IEnumerator<T> AsGeneric<T>(this IEnumerator iEnumerator)
    {
        while (iEnumerator.MoveNext())
        {
            yield return (T)iEnumerator.Current;
        }
    }

    /// <summary>
    /// Converts an <see cref="System.Collections.IEnumerator"/> instance to an <see cref="System.Collections.Generic.IEnumerator{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the generic enumerator.</typeparam>
    /// <param name="iEnumerator">The <see cref="System.Collections.IEnumerator"/>.</param>
    /// <returns>An instance of <see cref="System.Collections.Generic.IEnumerator{T}"/>.</returns>
    [Obsolete("Use AsGeneric<T>() instead. This method will be removed in a future version.")]
    public static IEnumerator<T> GetEnumerator<T>(this IEnumerator iEnumerator) => iEnumerator.AsGeneric<T>();
}
