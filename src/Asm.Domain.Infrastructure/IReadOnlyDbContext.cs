using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// A DB context interface that allows the reading of DbSets, but without save functionality.
/// </summary>
public interface IReadOnlyDbContext
{
    /// <summary>
    /// Gets a <see cref="DbSet{T}"/> for the given entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The set of entities.</returns>
    DbSet<T> Set<T>() where T : class;
}
