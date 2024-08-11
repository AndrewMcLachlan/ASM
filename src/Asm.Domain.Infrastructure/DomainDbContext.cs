using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// A <see cref="DbContext"/> that raises domain events.
/// </summary>
/// <param name="options">Options for the context.</param>
/// <param name="mediator">A mediator instance for orchestrating domain events.</param>
public abstract class DomainDbContext(DbContextOptions options, IMediator mediator) : DbContext(options), IUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainDbContext"/> class.
    /// </summary>
    /// <param name="mediator">A mediator instance for orchestrating domain events.</param>
    protected DomainDbContext(IMediator mediator) : this(new DbContextOptions<DbContext>(), mediator) { }

    /// <summary>
    /// Saves changes to the database and raises domain events.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">
    /// Indicates whether <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" />
    /// is called after the changes have been sent successfully to the database.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains
    /// the number of state entries written to the database.
    /// </returns>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    ///  An error is encountered while saving to the database.
    ///  </exception>
    ///  <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    ///  A concurrency violation is encountered while saving to the database. A concurrency
    ///  violation occurs when an unexpected number of rows are affected during save.
    ///  This is usually because the data in the database has been modified since it was
    ///  loaded into memory.
    ///  </exception>
    ///  <exception cref="System.OperationCanceledException">
    ///  If the <paramref name="cancellationToken"/> is cancelled.
    ///  </exception>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default)
    {
        var domainEventEntities = ChangeTracker.Entries<IEntity>()
            .Select(entry => entry.Entity)
            .Where(entry => entry.Events.Count != 0)
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();
            foreach (var domainEvent in events)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }
        }

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Saves changes to the database and raises domain events.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains
    /// the number of state entries written to the database.
    /// </returns>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    ///  An error is encountered while saving to the database.
    ///  </exception>
    ///  <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    ///  A concurrency violation is encountered while saving to the database. A concurrency
    ///  violation occurs when an unexpected number of rows are affected during save.
    ///  This is usually because the data in the database has been modified since it was
    ///  loaded into memory.
    ///  </exception>
    ///  <exception cref="System.OperationCanceledException">
    ///  If the <paramref name="cancellationToken"/> is cancelled.
    ///  </exception>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);

    /// <summary>
    /// Saves changes to the database and raises domain events.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">
    /// Indicates whether <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" />
    /// is called after the changes have been sent successfully to the database.
    /// </param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save.
    /// This is usually because the data in the database has been modified since it was loaded into memory.
    /// </exception>
    public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
    {
        var domainEventEntities = ChangeTracker.Entries<IEntity>()
            .Select(entry => entry.Entity)
            .Where(entry => entry.Events.Count != 0)
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();
            foreach (var domainEvent in events)
            {
                mediator.Publish(domainEvent).Wait();
            }
        }


        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
}
