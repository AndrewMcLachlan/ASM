using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    #region Commands
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly commandsAssembly)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(commandsAssembly);
        });

        services.TryAddTransient<ICommandDispatcher, MediatrCommandDispatcher>();

        return services;
    }

    public static IServiceCollection AddCommandHandler<THandler, TRequest, TResponse>(this IServiceCollection services) where THandler : class, ICommandHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
        services.AddMediatR(config => {});

        services.TryAddTransient<ICommandDispatcher, MediatrCommandDispatcher>();

        services.AddTransient<ICommandHandler<TRequest, TResponse>, THandler>();

        return services;
    }

    public static IServiceCollection AddCommandHandler<THandler, TRequest>(this IServiceCollection services) where THandler : class, ICommandHandler<TRequest> where TRequest : ICommand
    {
        services.AddMediatR(config => { });

        services.TryAddTransient<ICommandDispatcher, MediatrCommandDispatcher>();

        services.AddTransient<ICommandHandler<TRequest>, THandler>();

        return services;
    }
    #endregion

    #region Queries
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, Assembly QuerysAssembly)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(QuerysAssembly);
        });

        services.TryAddTransient<IQueryDispatcher, MediatrQueryDispatcher>();

        return services;
    }

    public static IServiceCollection AddQueryHandler<THandler, TRequest, TResponse>(this IServiceCollection services) where THandler : class, IQueryHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        services.AddMediatR(config => { });

        services.TryAddTransient<IQueryDispatcher, MediatrQueryDispatcher>();

        services.AddTransient<IQueryHandler<TRequest, TResponse>, THandler>();

        return services;
    }
    #endregion
}
