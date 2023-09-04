using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;
public abstract class DomainDbContext : DbContext, IUnitOfWork
{
    protected readonly IMediator _dispatcher;

    public DomainDbContext(DbContextOptions options, IMediator mediator) : base(options)
    {
        _dispatcher = mediator;
    }

    protected DomainDbContext(IMediator mediator) : this(new DbContextOptions<DbContext>(), mediator) { }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var domainEventEntities = ChangeTracker.Entries<IEntity>()
            .Select(entry => entry.Entity)
            .Where(entry => entry.Events.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();
            foreach (var domainEvent in events)
            {
                await _dispatcher.Publish(domainEvent, cancellationToken);
            }
        }

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges()
    {
        var domainEventEntities = ChangeTracker.Entries<IEntity>()
            .Select(entry => entry.Entity)
            .Where(entry => entry.Events.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();
            foreach (var domainEvent in events)
            {
                _dispatcher.Publish(domainEvent).RunSynchronously();
            }
        }


        return base.SaveChanges();
    }
}
