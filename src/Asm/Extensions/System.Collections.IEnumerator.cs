namespace System.Collections;

/// <summary>
/// Extension methods for the <see cref="System.Collections.IEnumerator"/> interface.
/// </summary>
public static class IEnumeratorExtensions
{
    /// <summary>
    /// Converts an <see cref="System.Collections.IEnumerator"/> instance to an <see cref="System.Collections.Generic.IEnumerator{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the generic enumerator.</typeparam>
    /// <param name="iEnumerator">The <see cref="System.Collections.IEnumerator"/>.</param>
    /// <returns>An instance of <see cref="System.Collections.Generic.IEnumerator{T}"/>.</returns>
    public static IEnumerator<T> GetEnumerator<T>(this IEnumerator iEnumerator)
    {
        while (iEnumerator.MoveNext())
        {
            yield return (T)iEnumerator.Current;
        }
    }
}
