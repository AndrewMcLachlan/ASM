using System.Reflection;
using Asm.Cqrs;
using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    // Cache generic type definitions to avoid repeated reflection calls
    private static readonly Type CommandHandlerGenericType = typeof(ICommandHandler<,>);
    private static readonly Type CommandHandlerVoidGenericType = typeof(ICommandHandler<>);
    private static readonly Type QueryHandlerGenericType = typeof(IQueryHandler<,>);

    #region Commands
    /// <summary>
    /// Adds command handlers from the given assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="commandsAssembly">The assembly containing the command handlers.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly commandsAssembly)
    {
        // Single pass: find and register in one go
        foreach (var type in commandsAssembly.DefinedTypes)
        {
            if (!type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
                continue;

            var commandHandlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           (i.GetGenericTypeDefinition() == CommandHandlerGenericType ||
                            i.GetGenericTypeDefinition() == CommandHandlerVoidGenericType))
                .ToArray();

            foreach (var interfaceType in commandHandlerInterfaces)
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient(interfaceType, type));
            }
        }

        services.TryAddTransient<ICommandDispatcher, Dispatcher>();
        services.AddLazyCache();

        return services;
    }

    /// <summary>
    /// Adds an individual command handler.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the command response.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddCommandHandler<THandler, TRequest, TResponse>(this IServiceCollection services) where THandler : class, ICommandHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<ICommandHandler<TRequest, TResponse>, THandler>());

        services.TryAddTransient<ICommandDispatcher, Dispatcher>();
        services.AddLazyCache();

        return services;
    }

    /// <summary>
    /// Adds an individual command handler.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddCommandHandler<THandler, TRequest>(this IServiceCollection services) where THandler : class, ICommandHandler<TRequest> where TRequest : ICommand
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<ICommandHandler<TRequest>, THandler>());

        services.TryAddTransient<ICommandDispatcher, Dispatcher>();
        services.AddLazyCache();

        return services;
    }
    #endregion

    #region Queries
    /// <summary>
    /// Adds query handlers
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="assembly"></param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, Assembly assembly)
    {
        // Single pass: find and register in one go
        foreach (var type in assembly.DefinedTypes)
        {
            if (!type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition) continue;

            var queryHandlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == QueryHandlerGenericType)
                .ToArray();

            foreach (var interfaceType in queryHandlerInterfaces)
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient(interfaceType, type));
            }
        }

        services.TryAddTransient<IQueryDispatcher, Dispatcher>();
        services.AddLazyCache();

        return services;
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddQueryHandler<THandler, TRequest, TResponse>(this IServiceCollection services) where THandler : class, IQueryHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IQueryHandler<TRequest, TResponse>, THandler>());

        services.TryAddTransient<IQueryDispatcher, Dispatcher>();
        services.AddLazyCache();

        return services;
    }
    #endregion
}
