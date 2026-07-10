using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain;

/// <summary>
/// An object that is identifiable by an <see cref="Id"/> property.
/// </summary>
/// <typeparam name="T">The type of the <see cref="Id"/>.</typeparam>
public interface IIdentifiable<T> : IEquatable<IIdentifiable<T>>
{
    /// <summary>
    /// Gets the ID.
    /// </summary>
    [DisallowNull]
    T Id { get; }

    /// <inheritdoc/>
    bool IEquatable<IIdentifiable<T>>.Equals(IIdentifiable<T>? other)
    {
        // Reflexive even for a default (transient) key.
        if (ReferenceEquals(this, other)) return true;

        // Different runtime types are never equal, even with the same key value.
        if (other is null || other.GetType() != GetType()) return false;

        // Two distinct transient entities (default key) are only equal by reference (handled above).
        if (EqualityComparer<T>.Default.Equals(Id, default!) || EqualityComparer<T>.Default.Equals(other.Id, default!)) return false;

        return EqualityComparer<T>.Default.Equals(other.Id, Id);
    }
}