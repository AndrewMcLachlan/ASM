using System.Collections.Concurrent;

namespace Asm.Hosting;

/// <summary>
/// A queue of background work items of type <typeparamref name="T"/>, produced by application code and
/// consumed by a <see cref="QueuedHostedService{T}"/>.
/// </summary>
/// <typeparam name="T">The type of work item.</typeparam>
public interface IBackgroundWorkQueue<T>
{
    /// <summary>
    /// Enqueues a work item for background processing.
    /// </summary>
    /// <param name="workItem">The work item to enqueue.</param>
    void Queue(T workItem);

    /// <summary>
    /// Waits for and removes the next work item.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels the wait.</param>
    /// <returns>The next work item.</returns>
    Task<T> DequeueAsync(CancellationToken cancellationToken);
}

/// <summary>
/// A thread-safe, in-memory <see cref="IBackgroundWorkQueue{T}"/> backed by a <see cref="ConcurrentQueue{T}"/>
/// and a <see cref="SemaphoreSlim"/>. Register it as a singleton (see
/// <c>AddBackgroundWorkQueue&lt;T&gt;()</c>) so producers and the hosted consumer share one instance.
/// </summary>
/// <typeparam name="T">The type of work item.</typeparam>
public sealed class BackgroundWorkQueue<T> : IBackgroundWorkQueue<T>, IDisposable
{
    private readonly ConcurrentQueue<T> _workItems = new();
    private readonly SemaphoreSlim _signal = new(0);
    private bool _disposed;

    /// <inheritdoc />
    public void Queue(T workItem)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _workItems.Enqueue(workItem);
        _signal.Release();
    }

    /// <inheritdoc />
    public async Task<T> DequeueAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out var workItem);
        return workItem!;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _signal.Dispose();
        _disposed = true;
    }
}
