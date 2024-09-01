using System.Linq.Expressions;

namespace Asm.Testing.Domain;

/// <summary>
/// An AsyncEnumerable that can be used to test async queries.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
/// <param name="expression">The expression.</param>
public class TestAsyncEnumerable<T>(Expression expression) : EnumerableQuery<T>(expression), IAsyncEnumerable<T>, IQueryable<T>
{
    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetEnumerator() =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}

/// <summary>
/// An AsyncEnumerator that can be used to test async queries.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
/// <param name="enumerator">The enumerator to asyncrify.</param>
public class TestAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> enumerator = enumerator ?? throw new ArgumentNullException();

    /// <inheritdoc />
    public T Current => enumerator.Current;

    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <inheritdoc />
    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(enumerator.MoveNext());
}
