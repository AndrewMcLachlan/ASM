using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;
public abstract class DomainDbContext(DbContextOptions options, IMediator mediator) : DbContext(options), IUnitOfWork
{
    protected DomainDbContext(IMediator mediator) : this(new DbContextOptions<DbContext>(), mediator) { }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
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

    public override int SaveChanges()
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


        return base.SaveChanges();
    }
}
