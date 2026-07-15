using Asm.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Hosting-related extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class AsmHostingServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers a singleton in-memory <see cref="IBackgroundWorkQueue{T}"/> for work items of type
        /// <typeparamref name="T"/>. Pair it with a <see cref="QueuedHostedService{T}"/> registered via
        /// <c>AddHostedService</c>.
        /// </summary>
        /// <typeparam name="T">The type of work item.</typeparam>
        /// <returns>The service collection, for chaining.</returns>
        public IServiceCollection AddBackgroundWorkQueue<T>() =>
            services.AddSingleton<IBackgroundWorkQueue<T>, BackgroundWorkQueue<T>>();
    }
}
