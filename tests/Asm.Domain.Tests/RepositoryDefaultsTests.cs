namespace Asm.Domain.Tests;

/// <summary>
/// Exercises the <em>default interface methods</em> on <see cref="IRepository{TEntity, TKey}"/> and
/// <see cref="IWritableRepository{TEntity, TKey}"/> through a hand-written in-memory repository that does
/// <em>not</em> override them.
/// </summary>
public class RepositoryDefaultsTests
{
    private sealed class InMemoryRepository : IWritableRepository<TestKeyedEntity, int>
    {
        private readonly List<TestKeyedEntity> _items = [];

        public IQueryable<TestKeyedEntity> Queryable() => _items.AsQueryable();

        public Task<IEnumerable<TestKeyedEntity>> Get(CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<TestKeyedEntity>>(_items);

        public Task<IEnumerable<TestKeyedEntity>> Get(ISpecification<TestKeyedEntity> specification, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<TestKeyedEntity>>(specification.Apply(_items.AsQueryable()).ToArray());

        public Task<TestKeyedEntity> Get(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_items.Single(e => e.Id == id));

        public Task<TestKeyedEntity> Get(int id, ISpecification<TestKeyedEntity> specification, CancellationToken cancellationToken = default) =>
            Task.FromResult(specification.Apply(_items.AsQueryable()).Single(e => e.Id == id));

        public TestKeyedEntity Add(TestKeyedEntity entity)
        {
            _items.Add(entity);
            return entity;
        }

        public TestKeyedEntity Update(TestKeyedEntity entity) => entity;

        // AddRange, Find and TryGet are deliberately NOT implemented, so the interface defaults run.
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddRange_Default_AddsEachEntity()
    {
        IWritableRepository<TestKeyedEntity, int> repository = new InMemoryRepository();

        repository.AddRange([new TestKeyedEntity(1), new TestKeyedEntity(2), new TestKeyedEntity(3)]);

        var all = await repository.Get(TestContext.Current.CancellationToken);
        Assert.Equal([1, 2, 3], all.Select(e => e.Id));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddRange_Default_NullEntities_Throws()
    {
        IWritableRepository<TestKeyedEntity, int> repository = new InMemoryRepository();

        Assert.Throws<ArgumentNullException>(() => repository.AddRange(null!));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Find_Default_ReturnsEntityWhenPresent()
    {
        IWritableRepository<TestKeyedEntity, int> repository = new InMemoryRepository();
        repository.AddRange([new TestKeyedEntity(1), new TestKeyedEntity(2)]);

        var found = await repository.Find(2, TestContext.Current.CancellationToken);

        Assert.NotNull(found);
        Assert.Equal(2, found.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Find_Default_ReturnsNullWhenAbsent()
    {
        IWritableRepository<TestKeyedEntity, int> repository = new InMemoryRepository();
        repository.AddRange([new TestKeyedEntity(1)]);

        var found = await repository.Find(99, TestContext.Current.CancellationToken);

        Assert.Null(found);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task TryGet_Default_ForwardsToFind()
    {
        IWritableRepository<TestKeyedEntity, int> repository = new InMemoryRepository();
        repository.AddRange([new TestKeyedEntity(5)]);

        Assert.NotNull(await repository.TryGet(5, TestContext.Current.CancellationToken));
        Assert.Null(await repository.TryGet(6, TestContext.Current.CancellationToken));
    }
}
