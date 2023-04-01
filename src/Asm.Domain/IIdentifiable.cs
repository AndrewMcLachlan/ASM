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
        if (other == null || Id!.Equals(default(T))) return false;

        return other.Id!.Equals(Id);
    }
}