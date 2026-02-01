using System.Linq.Expressions;

namespace Asm.Testing.Domain;

/// <summary>
/// An AsyncEnumerable that can be used to test async queries.
/// </summary>
/// <remarks>
/// <para>
/// This class wraps an expression-based queryable and provides async enumeration support.
/// It also overrides <see cref="IQueryable.Provider"/> to return a <see cref="TestAsyncQueryProvider{TEntity}"/>,
/// ensuring that chained LINQ operations (like <c>.Where().Select().SingleOrDefaultAsync()</c>)
/// continue to use the async-capable provider throughout the query chain.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of entity.</typeparam>
public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    private readonly IQueryProvider _asyncProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAsyncEnumerable{T}"/> class.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public TestAsyncEnumerable(Expression expression) : base(expression)
    {
        _asyncProvider = new TestAsyncQueryProvider<T>(((IQueryable<T>)new EnumerableQuery<T>(expression)).Provider);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAsyncEnumerable{T}"/> class.
    /// </summary>
    /// <param name="enumerable">The enumerable to wrap.</param>
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
    {
        _asyncProvider = new TestAsyncQueryProvider<T>(enumerable.AsQueryable().Provider);
    }

    /// <summary>
    /// Gets the query provider that is associated with this data source.
    /// </summary>
    /// <remarks>
    /// This returns a <see cref="TestAsyncQueryProvider{TEntity}"/> to ensure async operations
    /// work correctly throughout the query chain.
    /// </remarks>
    IQueryProvider IQueryable.Provider => _asyncProvider;

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}

/// <summary>
/// An AsyncEnumerator that can be used to test async queries.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
/// <param name="enumerator">The enumerator to wrap.</param>
public class TestAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));

    /// <inheritdoc />
    public T Current => _enumerator.Current;

    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <inheritdoc />
    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_enumerator.MoveNext());
}
