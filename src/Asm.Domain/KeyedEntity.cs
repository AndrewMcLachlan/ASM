using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain;

/// <summary>
/// Base class for a keyed entity.
/// </summary>
/// <typeparam name="TKey">The type of the entity key.</typeparam>
public abstract class KeyedEntity<TKey> : Entity, IIdentifiable<TKey>
{
    /// <summary>
    /// Gets the ID.
    /// </summary>
    [Key]
    [DisallowNull]
    public TKey Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyedEntity{TKey}"/> class.
    /// </summary>
    /// <param name="id">The ID.</param>
    protected KeyedEntity([DisallowNull] TKey id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => ((IEquatable<IIdentifiable<TKey>>)this).Equals(obj as IIdentifiable<TKey>);

    /// <inheritdoc/>
    public static bool operator ==(KeyedEntity<TKey>? a, KeyedEntity<TKey>? b) => (a is null && b is null) || (a?.Equals(b) ?? false);

    /// <inheritdoc/>
    public static bool operator !=(KeyedEntity<TKey>? a, KeyedEntity<TKey>? b) => !(a == b);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Id!.GetHashCode();
    }
}