using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// A <see cref="DbContext"/> that raises domain events.
/// </summary>
/// <param name="options">Options for the context.</param>
/// <param name="publisher">A publisher instance for orchestrating domain events.</param>
public abstract class DomainDbContext(DbContextOptions options, IPublisher publisher) : DbContext(options), IUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainDbContext"/> class.
    /// </summary>
    /// <param name="publisher">A publisher instance for orchestrating domain events.</param>
    protected DomainDbContext(IPublisher publisher) : this(new DbContextOptions<DbContext>(), publisher) { }

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
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        // Phase 1: drain and dispatch pre-save handlers inside the transaction. The returned snapshot
        // is every event dispatched (including those raised by pre-save handlers during the drain),
        // captured before entity.Events was cleared so it survives into the post-save phase.
        var dispatchedEvents = await DispatchPreSaveDomainEventsAsync(cancellationToken).ConfigureAwait(false);

        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);

        // Phase 2: the commit succeeded, so run the post-save handlers against the captured snapshot.
        await DispatchPostSaveDomainEventsAsync(dispatchedEvents, cancellationToken).ConfigureAwait(false);

        return result;
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
        // Phase 1: drain and dispatch pre-save handlers, capturing the snapshot. SaveChanges has no
        // cancellation token, so none is propagated here.
        List<IDomainEvent> dispatchedEvents = [];

        foreach (var _ in Bounded.While(MaxDomainEventDrainIterations))
        {
            var domainEventEntities = ChangeTracker.Entries<IEntity>()
                .Select(entry => entry.Entity)
                .Where(entry => entry.Events.Count != 0)
                .ToArray();

            if (domainEventEntities.Length == 0)
            {
                break;
            }

            foreach (var entity in domainEventEntities)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    dispatchedEvents.Add(domainEvent);
                    publisher.PublishPreSave(domainEvent).AsTask().GetAwaiter().GetResult();
                }
            }
        }

        var result = base.SaveChanges(acceptAllChangesOnSuccess);

        // Phase 2: the commit succeeded, so run the post-save handlers against the captured snapshot.
        foreach (var domainEvent in dispatchedEvents)
        {
            publisher.PublishPostSave(domainEvent).AsTask().GetAwaiter().GetResult();
        }

        return result;
    }

    /// <summary>
    /// The maximum number of dispatch rounds allowed while draining domain events. Cascades are
    /// normally one or two rounds deep; exceeding this indicates a handler raising events without
    /// end.
    /// </summary>
    private const int MaxDomainEventDrainIterations = 100;

    private async Task<List<IDomainEvent>> DispatchPreSaveDomainEventsAsync(CancellationToken cancellationToken)
    {
        // Drain until no new events remain: a pre-save handler may itself raise events (or add tracked
        // entities that carry events), and those must be dispatched in this same save. Bounded so a
        // handler that raises events indefinitely fails fast rather than hanging. Every dispatched
        // event is captured up front (before entity.Events is cleared) so the post-save phase can
        // re-read the whole set — including events raised by pre-save handlers, which commit in the
        // same transaction and therefore have their post-save handlers fired too.
        List<IDomainEvent> dispatchedEvents = [];

        foreach (var _ in Bounded.While(MaxDomainEventDrainIterations))
        {
            var domainEventEntities = ChangeTracker.Entries<IEntity>()
                .Select(entry => entry.Entity)
                .Where(entry => entry.Events.Count != 0)
                .ToArray();

            if (domainEventEntities.Length == 0)
            {
                break;
            }

            foreach (var entity in domainEventEntities)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    dispatchedEvents.Add(domainEvent);
                    await publisher.PublishPreSave(domainEvent, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        return dispatchedEvents;
    }

    private async Task DispatchPostSaveDomainEventsAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        // Post-save handlers run after a successful commit. A handler that throws here runs against an
        // already-committed aggregate — there is no rollback — so post-save handlers must be
        // idempotent and safe to retry. Guaranteed delivery is outbox territory and is out of scope.
        foreach (var domainEvent in domainEvents)
        {
            await publisher.PublishPostSave(domainEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
