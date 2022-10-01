namespace Asm.Extensions;

public static class ArrayExtensions
{
    public static T[] Shuffle<T>(this T[] array)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
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
}
