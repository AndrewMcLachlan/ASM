using System.Reflection;
using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    #region Commands
    /// <summary>
    /// Adds command handlers from the given assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="commandsAssembly">The assembly containing the command handlers.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly commandsAssembly)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(commandsAssembly);
        });

        services.TryAddTransient<ICommandDispatcher, MediatRCommandDispatcher>();

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
        services.AddMediatR(config => { });

        services.TryAddTransient<ICommandDispatcher, MediatRCommandDispatcher>();

        services.AddTransient<ICommandHandler<TRequest, TResponse>, THandler>();

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
        services.AddMediatR(config => { });

        services.TryAddTransient<ICommandDispatcher, MediatRCommandDispatcher>();

        services.AddTransient<ICommandHandler<TRequest>, THandler>();

        return services;
    }
    #endregion

    #region Queries
    /// <summary>
    /// Adds query handlers
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="QueriesAssembly"></param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, Assembly QueriesAssembly)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(QueriesAssembly);
        });

        services.TryAddTransient<IQueryDispatcher, MediatRQueryDispatcher>();

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
        services.AddMediatR(config => { });

        services.TryAddTransient<IQueryDispatcher, MediatRQueryDispatcher>();

        services.AddTransient<IQueryHandler<TRequest, TResponse>, THandler>();

        return services;
    }
    #endregion
}
