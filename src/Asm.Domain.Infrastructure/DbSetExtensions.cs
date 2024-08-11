using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

internal static class DbSetExtensions
{
    /// <summary>
    /// Finds an entity with the given primary key. If an entity with the given primary key
    /// is being tracked by the context, then it is returned immediately without making a request to the
    /// database. Otherwise, a query is made to the database for an entity with the given primary key
    /// and this entity, if found, is attached to the context and returned. If no entity is found, then
    /// null is returned.
    /// </summary>
    /// <remarks>
    /// See <see href="https://aka.ms/efcore-docs-find">Using Find and FindAsync</see> for more information and examples.
    /// </remarks>
    /// <typeparam name="TEntity">The type of entity to find.</typeparam>s
    /// <param name="source">The <see cref="DbSet{TEntity}" /> to find the entity in.</param>
    /// <param name="keyValue">The values of the primary key for the entity to be found.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is cancelled.</exception>
    public static ValueTask<TEntity?> FindAsync<TEntity>(this DbSet<TEntity> source, object keyValue, CancellationToken cancellationToken = default) where TEntity : class =>
        source.FindAsync([keyValue], cancellationToken);
}
