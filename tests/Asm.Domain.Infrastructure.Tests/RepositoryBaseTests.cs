using Asm;
using Asm.Domain;
using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure.Tests;

/// <summary>
/// Behaviour tests for <see cref="RepositoryBase{TContext, TEntity, TKey}"/> and
/// <see cref="RepositoryWriteBase{TContext, TEntity, TKey}"/> against an in-memory database, covering
/// <c>Get</c> vs <c>Find</c>/<c>TryGet</c> semantics, <c>AddRange</c>, and reference-type keys enabled by
/// the relaxed <c>notnull</c> constraint.
/// </summary>
public class RepositoryBaseTests
{
    private sealed class Widget : KeyedEntity<int>
    {
        public Widget() : base(0) { }

        public Widget(int id, string name) : base(id) => Name = name;

        public string Name { get; set; } = String.Empty;
    }

    // A string-keyed entity: only compilable because TKey is now constrained to notnull (not struct).
    private sealed class Gadget : KeyedEntity<string>
    {
        public Gadget() : base(String.Empty) { }

        public Gadget(string id, string label) : base(id) => Label = label;

        public string Label { get; set; } = String.Empty;
    }

    private sealed class RepoDbContext(DbContextOptions options, IPublisher publisher) : DomainDbContext(options, publisher)
    {
        public DbSet<Widget> Widgets { get; set; } = null!;

        public DbSet<Gadget> Gadgets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Widget>().HasKey(w => w.Id);
            modelBuilder.Entity<Gadget>().HasKey(g => g.Id);
        }
    }

    private sealed class WidgetRepository(RepoDbContext context) : RepositoryWriteBase<RepoDbContext, Widget, int>(context);

    private sealed class GadgetRepository(RepoDbContext context) : RepositoryWriteBase<RepoDbContext, Gadget, string>(context);

    private sealed class NoOpPublisher : IPublisher
    {
        public ValueTask PublishPreSave<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent =>
            ValueTask.CompletedTask;

        public ValueTask PublishPostSave<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent =>
            ValueTask.CompletedTask;
    }

    private static RepoDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<RepoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, new NoOpPublisher());

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Get_ByKey_ReturnsEntity()
    {
        var token = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        context.Widgets.Add(new Widget(1, "One"));
        await context.SaveChangesAsync(token);

        var repository = new WidgetRepository(context);
        var widget = await repository.Get(1, token);

        Assert.Equal("One", widget.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Get_MissingKey_ThrowsNotFoundException()
    {
        var token = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        var repository = new WidgetRepository(context);

        await Assert.ThrowsAsync<NotFoundException>(() => repository.Get(42, token));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Find_MissingKey_ReturnsNull()
    {
        var token = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        var repository = new WidgetRepository(context);

        Assert.Null(await repository.Find(42, token));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Find_ExistingKey_ReturnsEntity()
    {
        var token = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        context.Widgets.Add(new Widget(7, "Seven"));
        await context.SaveChangesAsync(token);

        var repository = new WidgetRepository(context);
        var widget = await repository.Find(7, token);

        Assert.NotNull(widget);
        Assert.Equal("Seven", widget.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task TryGet_MatchesFindSemantics()
    {
        var token = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        context.Widgets.Add(new Widget(3, "Three"));
        await context.SaveChangesAsync(token);

        var repository = new WidgetRepository(context);

        Assert.NotNull(await repository.TryGet(3, token));
        Assert.Null(await repository.TryGet(99, token));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddRange_PersistsAllEntities()
    {
        var token = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        var repository = new WidgetRepository(context);

        repository.AddRange([new Widget(1, "One"), new Widget(2, "Two"), new Widget(3, "Three")]);
        await context.SaveChangesAsync(token);

        var all = await repository.Get(token);
        Assert.Equal([1, 2, 3], all.Select(w => w.Id).OrderBy(id => id));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReferenceTypeKey_IsSupported()
    {
        var token = TestContext.Current.CancellationToken;
        using var context = CreateContext();
        context.Gadgets.Add(new Gadget("abc", "Alpha"));
        await context.SaveChangesAsync(token);

        var repository = new GadgetRepository(context);

        var found = await repository.Get("abc", token);
        Assert.Equal("Alpha", found.Label);

        Assert.Null(await repository.Find("missing", token));
        await Assert.ThrowsAsync<NotFoundException>(() => repository.Get("missing", token));
    }
}
