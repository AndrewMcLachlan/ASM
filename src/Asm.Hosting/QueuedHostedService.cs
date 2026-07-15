using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Asm.Hosting;

/// <summary>
/// A <see cref="BackgroundService"/> that consumes work items from an <see cref="IBackgroundWorkQueue{T}"/>,
/// processing each within its own dependency-injection scope. Derive from this and implement
/// <see cref="ProcessAsync"/>; the base class owns the dequeue loop, per-item scoping, and error logging
/// (a failing item is logged and the loop continues).
/// </summary>
/// <typeparam name="T">The type of work item.</typeparam>
/// <param name="workQueue">The queue to consume from.</param>
/// <param name="serviceScopeFactory">Factory used to create a scope per work item.</param>
/// <param name="loggerFactory">Factory used to create the logger, categorised by the derived type.</param>
public abstract class QueuedHostedService<T>(IBackgroundWorkQueue<T> workQueue, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory) : BackgroundService
{
    /// <summary>
    /// Processes a single work item. A fresh DI scope is created per item and supplied as
    /// <paramref name="services"/>; resolve scoped dependencies from it.
    /// </summary>
    /// <param name="services">The scoped service provider for this work item.</param>
    /// <param name="workItem">The work item to process.</param>
    /// <param name="cancellationToken">A token signalled when the host is stopping.</param>
    protected abstract ValueTask ProcessAsync(IServiceProvider services, T workItem, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a value describing <paramref name="workItem"/> to include in the error log when
    /// <see cref="ProcessAsync"/> throws — typically a non-sensitive identifier such as an id.
    /// </summary>
    /// <remarks>
    /// Returns <c>null</c> by default, so <b>no work-item content is logged</b> unless a derived class
    /// opts in. The base class cannot know which parts of <typeparamref name="T"/> are sensitive, so the
    /// safe default is to log nothing about the item. When overriding, return only values that are safe to
    /// appear in logs — never personal data, secrets, or payloads (e.g. an imported file's contents).
    /// </remarks>
    /// <param name="workItem">The work item that failed.</param>
    /// <returns>A log-safe descriptor, or <c>null</c> to omit the item from the error log.</returns>
    protected virtual object? DescribeWorkItem(T workItem) => null;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var logger = loggerFactory.CreateLogger(GetType());
        logger.LogInformation("{Service} is starting.", GetType().Name);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await workQueue.DequeueAsync(stoppingToken);

                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    await ProcessAsync(scope.ServiceProvider, workItem, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // Only log a descriptor the derived class has explicitly deemed safe (default: none),
                    // so sensitive parts of the work item never leak into logs.
                    var description = DescribeWorkItem(workItem);
                    if (description is null)
                    {
                        logger.LogError(ex, "Error processing a work item in {Service}.", GetType().Name);
                    }
                    else
                    {
                        logger.LogError(ex, "Error processing work item {WorkItem} in {Service}.", description, GetType().Name);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown — the stopping token was signalled.
        }

        logger.LogInformation("{Service} is stopping.", GetType().Name);
    }
}
