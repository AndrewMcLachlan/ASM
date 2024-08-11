namespace Asm.Domain;

/// <summary>
/// A unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Save changes to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Save changes to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save changes to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    int SaveChanges(bool acceptAllChangesOnSuccess = true);
}
