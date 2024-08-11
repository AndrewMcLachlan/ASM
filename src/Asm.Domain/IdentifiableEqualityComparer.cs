using System.Diagnostics.CodeAnalysis;

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
        if (x == null || x.Id == null || y == null || y.Id == null) return false;

        return x.Id.Equals(y.Id);
    }

    /// <inheritdoc />
    public override int GetHashCode([DisallowNull] TType obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return obj.Id!.GetHashCode();
    }
}