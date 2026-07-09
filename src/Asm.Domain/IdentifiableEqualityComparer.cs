namespace Asm.Domain;

/// <summary>
/// An equality comparer for <see cref="IIdentifiable{T}"/> objects.
/// </summary>
/// <typeparam name="TType">The type to compare.</typeparam>
/// <typeparam name="TKey">The type of the identifiable item's key.</typeparam>
public class IIdentifiableEqualityComparer<TType, TKey> : EqualityComparer<TType> where TType : IIdentifiable<TKey>
{
    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <remarks>
    /// The comparison is based on the <see cref="IIdentifiable{T}.Id"/> property.
    /// </remarks>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns></returns>
    public override bool Equals(TType? x, TType? y)
    {
        // Matches the IEqualityComparer contract: two nulls (or the same reference) are equal.
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return EqualityComparer<TKey>.Default.Equals(x.Id, y.Id);
    }

    /// <inheritdoc />
    public override int GetHashCode(TType obj)
    {
        // The contract allows GetHashCode(null); return 0 rather than throwing.
        if (obj is null || obj.Id is null) return 0;
        return obj.Id.GetHashCode();
    }
}