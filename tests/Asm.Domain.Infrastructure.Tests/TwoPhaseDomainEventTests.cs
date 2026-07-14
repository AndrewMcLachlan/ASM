using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Asm.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Asm.Domain.Infrastructure.Tests;

/// <summary>
/// Behavioural tests for the two-phase (pre-save / post-save) domain-event dispatch performed by
/// <see cref="DomainDbContext"/> and <see cref="Publisher"/>.
/// </summary>
public class TwoPhaseDomainEventTests
{
    #region Test Types

    private sealed record OrderPlaced(int OrderId) : IDomainEvent;

    private sealed record OrderConfirmed(int OrderId) : IDomainEvent;

    private sealed class Order : IEntity
    {
        [Key]
        public int Id { get; set; }

        public ICollection<IDomainEvent> Events { get; } = [];
    }

    private sealed class AuditEntry
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; } = String.Empty;
    }

    private sealed class Recorder
    {
        public List<string> Calls { get; } = [];

        public bool OrderPersistedDuringPostSave { get; set; }
    }

    private sealed class PhaseDbContext(DbContextOptions options, IPublisher publisher) : DomainDbContext(options, publisher)
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<AuditEntry> Audits { get; set; }
    }

    /// <summary>Pre-save handler that writes an audit row into the same transaction.</summary>
    private sealed class PreSaveAuditHandler(PhaseDbContext context, Recorder recorder) : IPreSaveDomainEventHandler<OrderPlaced>
    {
        public ValueTask Handle(OrderPlaced domainEvent, CancellationToken cancellationToken = default)
        {
            recorder.Calls.Add("pre:OrderPlaced");
            context.Audits.Add(new AuditEntry { Message = "placed" });
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Post-save handler that observes whether the aggregate is already committed.</summary>
    private sealed class PostSaveNotifyHandler(PhaseDbContext context, Recorder recorder) : IPostSaveDomainEventHandler<OrderPlaced>
    {
        public ValueTask Handle(OrderPlaced domainEvent, CancellationToken cancellationToken = default)
        {
            recorder.Calls.Add("post:OrderPlaced");
            recorder.OrderPersistedDuringPostSave = context.Orders.Any(o => o.Id == domainEvent.OrderId);
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Handler that opts into both phases with a single implementation.</summary>
    private sealed class BothPhasesHandler(Recorder recorder) : IPreSaveDomainEventHandler<OrderConfirmed>, IPostSaveDomainEventHandler<OrderConfirmed>
    {
        public ValueTask Handle(OrderConfirmed domainEvent, CancellationToken cancellationToken = default)
        {
            recorder.Calls.Add("both:OrderConfirmed");
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Pre-save handler that raises a further event during the drain.</summary>
    private sealed class RaiseOnPreSaveHandler(PhaseDbContext context, Recorder recorder) : IPreSaveDomainEventHandler<OrderPlaced>
    {
        public ValueTask Handle(OrderPlaced domainEvent, CancellationToken cancellationToken = default)
        {
            recorder.Calls.Add("pre:OrderPlaced");
            var order = context.Set<Order>().Local.Single(o => o.Id == domainEvent.OrderId);
            order.Events.Add(new OrderConfirmed(order.Id));
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Post-save handler for the cascaded event.</summary>
    private sealed class PostSaveConfirmedHandler(Recorder recorder) : IPostSaveDomainEventHandler<OrderConfirmed>
    {
        public ValueTask Handle(OrderConfirmed domainEvent, CancellationToken cancellationToken = default)
        {
            recorder.Calls.Add("post:OrderConfirmed");
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Legacy handler implementing only the base <see cref="IDomainEventHandler{TDomainEvent}"/>.</summary>
    private sealed class LegacyOrderPlacedHandler(Recorder recorder) : IDomainEventHandler<OrderPlaced>
    {
        public ValueTask Handle(OrderPlaced domainEvent, CancellationToken cancellationToken = default)
        {
            recorder.Calls.Add("legacy:OrderPlaced");
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Pre-save handler that adds an audit row colliding with a pre-existing key.</summary>
    private sealed class DuplicateKeyPreSaveHandler(PhaseDbContext context, Recorder recorder) : IPreSaveDomainEventHandler<OrderPlaced>
    {
        public ValueTask Handle(OrderPlaced domainEvent, CancellationToken cancellationToken = default)
        {
            recorder.Calls.Add("pre:OrderPlaced");
            context.Audits.Add(new AuditEntry { Id = 1, Message = "duplicate" });
            return ValueTask.CompletedTask;
        }
    }

    #endregion

    private static ServiceProvider BuildProvider(params Type[] handlerTypes)
    {
        var services = new ServiceCollection();
        services.AddSingleton<Recorder>();
        services.AddDbContext<PhaseDbContext>(options => options.UseInMemoryDatabase($"phase_{Guid.NewGuid()}"));

        var assembly = new Mock<Assembly>();
        assembly.Setup(a => a.DefinedTypes).Returns(handlerTypes.Select(t => t.GetTypeInfo()).ToList());
        services.AddDomainEvents(assembly.Object);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Given a pre-save handler that writes to the context,
    /// when SaveChanges runs,
    /// then the handler runs before the write and its change is persisted in the same transaction.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void PreSaveHandlerRunsBeforeSaveAndChangesPersist()
    {
        using var provider = BuildProvider(typeof(PreSaveAuditHandler));
        var recorder = provider.GetRequiredService<Recorder>();
        var context = provider.GetRequiredService<PhaseDbContext>();

        var order = new Order { Id = 42 };
        order.Events.Add(new OrderPlaced(order.Id));
        context.Orders.Add(order);

        context.SaveChanges();

        Assert.Contains("pre:OrderPlaced", recorder.Calls);
        Assert.Equal("placed", context.Audits.Single().Message);
    }

    /// <summary>
    /// Given a post-save handler,
    /// when SaveChangesAsync commits successfully,
    /// then the handler runs after the commit and sees the aggregate already persisted.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task PostSaveHandlerRunsAfterSuccessfulCommit()
    {
        using var provider = BuildProvider(typeof(PostSaveNotifyHandler));
        var recorder = provider.GetRequiredService<Recorder>();
        var context = provider.GetRequiredService<PhaseDbContext>();

        var order = new Order { Id = 7 };
        order.Events.Add(new OrderPlaced(order.Id));
        context.Orders.Add(order);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Contains("post:OrderPlaced", recorder.Calls);
        Assert.True(recorder.OrderPersistedDuringPostSave);
    }

    /// <summary>
    /// Given a post-save handler and a save that fails,
    /// when SaveChangesAsync throws,
    /// then the pre-save handler has run but the post-save handler is never invoked.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task PostSaveHandlerNotCalledWhenSaveThrows()
    {
        using var provider = BuildProvider(typeof(DuplicateKeyPreSaveHandler), typeof(PostSaveNotifyHandler));
        var recorder = provider.GetRequiredService<Recorder>();
        var context = provider.GetRequiredService<PhaseDbContext>();

        // Seed a row so the pre-save handler's insert collides on the primary key at write time.
        context.Audits.Add(new AuditEntry { Id = 1, Message = "seed" });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var order = new Order { Id = 7 };
        order.Events.Add(new OrderPlaced(order.Id));
        context.Orders.Add(order);

        await Assert.ThrowsAnyAsync<Exception>(() => context.SaveChangesAsync(TestContext.Current.CancellationToken));

        Assert.Contains("pre:OrderPlaced", recorder.Calls);
        Assert.DoesNotContain("post:OrderPlaced", recorder.Calls);
    }

    /// <summary>
    /// Given a post-save handler and a synchronous save that fails,
    /// when SaveChanges throws,
    /// then the pre-save handler has run but the post-save handler is never invoked.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void PostSaveHandlerNotCalledWhenSyncSaveThrows()
    {
        using var provider = BuildProvider(typeof(DuplicateKeyPreSaveHandler), typeof(PostSaveNotifyHandler));
        var recorder = provider.GetRequiredService<Recorder>();
        var context = provider.GetRequiredService<PhaseDbContext>();

        context.Audits.Add(new AuditEntry { Id = 1, Message = "seed" });
        context.SaveChanges();

        var order = new Order { Id = 7 };
        order.Events.Add(new OrderPlaced(order.Id));
        context.Orders.Add(order);

        Assert.ThrowsAny<Exception>(() => context.SaveChanges());

        Assert.Contains("pre:OrderPlaced", recorder.Calls);
        Assert.DoesNotContain("post:OrderPlaced", recorder.Calls);
    }

    /// <summary>
    /// Given a handler implementing both phase interfaces,
    /// when SaveChangesAsync runs,
    /// then the handler is invoked once in the pre-save phase and once in the post-save phase.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandlerImplementingBothPhasesRunsInBothPhases()
    {
        using var provider = BuildProvider(typeof(BothPhasesHandler));
        var recorder = provider.GetRequiredService<Recorder>();
        var context = provider.GetRequiredService<PhaseDbContext>();

        var order = new Order { Id = 3 };
        order.Events.Add(new OrderConfirmed(order.Id));
        context.Orders.Add(order);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, recorder.Calls.Count(c => c == "both:OrderConfirmed"));
    }

    /// <summary>
    /// Given a pre-save handler that raises a further event during the drain,
    /// when SaveChangesAsync commits,
    /// then the event it raised is also delivered to post-save handlers.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task EventRaisedByPreSaveHandlerIsDeliveredPostSave()
    {
        using var provider = BuildProvider(typeof(RaiseOnPreSaveHandler), typeof(PostSaveConfirmedHandler));
        var recorder = provider.GetRequiredService<Recorder>();
        var context = provider.GetRequiredService<PhaseDbContext>();

        var order = new Order { Id = 9 };
        order.Events.Add(new OrderPlaced(order.Id));
        context.Orders.Add(order);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Contains("post:OrderConfirmed", recorder.Calls);
    }

    /// <summary>
    /// Given a handler implementing only the legacy <see cref="IDomainEventHandler{TDomainEvent}"/>,
    /// when SaveChanges runs,
    /// then the handler still runs in the pre-save phase.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void LegacyDomainEventHandlerStillRunsPreSave()
    {
        using var provider = BuildProvider(typeof(LegacyOrderPlacedHandler));
        var recorder = provider.GetRequiredService<Recorder>();
        var context = provider.GetRequiredService<PhaseDbContext>();

        var order = new Order { Id = 1 };
        order.Events.Add(new OrderPlaced(order.Id));
        context.Orders.Add(order);

        context.SaveChanges();

        Assert.Contains("legacy:OrderPlaced", recorder.Calls);
    }
}
