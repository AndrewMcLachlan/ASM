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
                    logger.LogError(ex, "Error processing work item {WorkItem} in {Service}.", workItem, GetType().Name);
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
