using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain;

/// <summary>
/// An equality comparer for entities that implement <see cref="KeyedEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TType">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public class KeyedEntityEqualityComparer<TType, TKey> : EqualityComparer<TType> where TType : KeyedEntity<TKey>
{
    /// <summary>
    /// Checks if two entities are equal.
    /// </summary>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns></returns>
    public override bool Equals(TType? x, TType? y) =>
        x == y;

    /// <inheritdoc/>
    public override int GetHashCode([DisallowNull] TType obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return obj.Id!.GetHashCode();
    }
}
