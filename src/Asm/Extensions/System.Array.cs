namespace System;

/// <summary>
/// Extensions for the <see cref="Array"/> class.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Shuffles the elements of an array.
    /// </summary>
    /// <typeparam name="T">The array type.</typeparam>
    /// <param name="array">The array to shuffle.</param>
    /// <returns>The same array with the elements reordered.</returns>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the array is read-only.</exception>
    public static T[] Shuffle<T>(this T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (array.IsReadOnly) throw new InvalidOperationException("Array is readonly");

        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (array[n], array[k]) = (array[k], array[n]);
        }

        return array;
    }

    /// <summary>
    /// Determines whether a array is <c>null</c> or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to check.</param>
    /// <returns><c>true</c> if the array is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty<T>(this T[]? array) =>
        (array?.Length ?? 0) == 0;

    /// <summary>
    /// Determines whether a array is empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to check.</param>
    /// <returns><c>true</c> if the array is empty; otherwise, <c>false</c>.</returns>
    public static bool Empty<T>(this T[] array) =>
        array.Length == 0;
}
