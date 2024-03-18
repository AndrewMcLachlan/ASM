using System.Reflection;
using Asm.Domain;
using Asm.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class AsmDomainInfrastructureIServiceCollectionExtensions
{
    private static readonly Type IQueryableType = typeof(IQueryable<>);
    private static readonly MethodInfo DbContextSetMethod = typeof(IReadOnlyDbContext).GetTypeInfo().GetDeclaredMethod(nameof(IReadOnlyDbContext.Set))!;
    private static readonly MethodInfo AsNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethod(nameof(EntityFrameworkQueryableExtensions.AsNoTracking))!;

    public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services) where T : class, IUnitOfWork =>
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<T>());


    public static IServiceCollection AddQueryable<TEntity, TContext>(this IServiceCollection services) where TEntity : class where TContext : IReadOnlyDbContext =>
        services.AddTransient(sp => sp.GetRequiredService<TContext>().Set<TEntity>().AsNoTracking());

    public static IServiceCollection AddAggregateRoots<TContext>(this IServiceCollection services, Assembly entityAssembly) where TContext : IReadOnlyDbContext
    {
        var types = entityAssembly.GetTypes().Where(t => t.CustomAttributes.Any(ca => ca.AttributeType == typeof(AggregateRootAttribute)));

        foreach (var type in types)
        {
            var queryableGeneric = IQueryableType.MakeGenericType(type);

            services.Add(new ServiceDescriptor(queryableGeneric, sp =>
            {
                IReadOnlyDbContext context = sp.GetRequiredService<TContext>();

                var genericSet = DbContextSetMethod.MakeGenericMethod(type);

                var res = genericSet.Invoke(context, null);

                var genericAsNoTracking = AsNoTrackingMethod.MakeGenericMethod(type);

                var res2 = genericAsNoTracking.Invoke(null, new[] { res });

                return res2!;
            }, ServiceLifetime.Transient));
        }
        return services;
    }

    public static IServiceCollection AddDomainEvents(this IServiceCollection services, Assembly domainEventsAssembly) =>
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(domainEventsAssembly);
        });

    public static IServiceCollection AddDomainEvent<THandler, TRequest>(this IServiceCollection services) where THandler : class, IDomainEventHandler<TRequest> where TRequest : IDomainEvent
    {
        services.AddMediatR(config => { });

        services.AddTransient<IDomainEventHandler<TRequest>, THandler>();

        return services;
    }

    #region Add Readonly DB Context
    /// <summary>
    ///     <para>
    ///         Registers the given context as a service in the <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure a connection string and other options.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same <see cref="DbContext" />
    ///         instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-di">Using DbContext with dependency injection</see> for more information.
    ///     </para>
    /// </summary>
    /// <typeparam name="TContext">The type of context to be registered.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="optionsAction">
    ///     <para>
    ///         An optional action to configure the <see cref="DbContextOptions" /> for the context. This provides an
    ///         alternative to performing configuration of the context by overriding the
    ///         <see cref="DbContext.OnConfiguring" /> method in your derived context.
    ///     </para>
    ///     <para>
    ///         If an action is supplied here, the <see cref="DbContext.OnConfiguring" /> method will still be run if it has
    ///         been overridden on the derived context. <see cref="DbContext.OnConfiguring" /> configuration will be applied
    ///         in addition to configuration performed here.
    ///     </para>
    ///     <para>
    ///         In order for the options to be passed into your context, you need to expose a constructor on your context that takes
    ///         <see cref="DbContextOptions{TContext}" /> and passes it to the base constructor of <see cref="DbContext" />.
    ///     </para>
    /// </param>
    /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
    /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddReadOnlyDbContext<TContext>(
        this IServiceCollection serviceCollection,
        Action<DbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContext : DbContext, IReadOnlyDbContext
        => AddReadOnlyDbContext<IReadOnlyDbContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);

    /// <summary>
    ///     <para>
    ///         Registers the given context as a service in the <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure a connection string and other options.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same <see cref="DbContext" />
    ///         instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-di">Using DbContext with dependency injection</see> for more information.
    ///     </para>
    /// </summary>
    /// <typeparam name="TContextService">The class or interface that will be used to resolve the context from the container.</typeparam>
    /// <typeparam name="TContextImplementation">The concrete implementation type to create.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="optionsAction">
    ///     <para>
    ///         An optional action to configure the <see cref="DbContextOptions" /> for the context. This provides an
    ///         alternative to performing configuration of the context by overriding the
    ///         <see cref="DbContext.OnConfiguring" /> method in your derived context.
    ///     </para>
    ///     <para>
    ///         If an action is supplied here, the <see cref="DbContext.OnConfiguring" /> method will still be run if it has
    ///         been overridden on the derived context. <see cref="DbContext.OnConfiguring" /> configuration will be applied
    ///         in addition to configuration performed here.
    ///     </para>
    ///     <para>
    ///         In order for the options to be passed into your context, you need to expose a constructor on your context that takes
    ///         <see cref="DbContextOptions{TContext}" /> and passes it to the base constructor of <see cref="DbContext" />.
    ///     </para>
    /// </param>
    /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
    /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddReadOnlyDbContext<TContextService, TContextImplementation>(
        this IServiceCollection serviceCollection,
        Action<DbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContextImplementation : DbContext, TContextService
        where TContextService : IReadOnlyDbContext
        => AddReadOnlyDbContext<TContextService, TContextImplementation>(
            serviceCollection,
            optionsAction == null
                ? null
                : (p, b) => optionsAction(b), contextLifetime, optionsLifetime);

    /// <summary>
    ///     <para>
    ///         Registers the given context as a service in the <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure a connection string and other options.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same <see cref="DbContext" />
    ///         instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-di">Using DbContext with dependency injection</see> for more information.
    ///     </para>
    /// </summary>
    /// <typeparam name="TContext">The type of context to be registered.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
    /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddReadOnlyDbContext<TContext>(
        this IServiceCollection serviceCollection,
        ServiceLifetime contextLifetime,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContext : DbContext, IReadOnlyDbContext
        => AddReadOnlyDbContext<IReadOnlyDbContext, TContext>(serviceCollection, contextLifetime, optionsLifetime);

    /// <summary>
    ///     <para>
    ///         Registers the given context as a service in the <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure a connection string and other options.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same <see cref="DbContext" />
    ///         instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-di">Using DbContext with dependency injection</see> for more information.
    ///     </para>
    /// </summary>
    /// <typeparam name="TContextService">The class or interface that will be used to resolve the context from the container.</typeparam>
    /// <typeparam name="TContextImplementation">The concrete implementation type to create.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
    /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddReadOnlyDbContext<TContextService, TContextImplementation>(
        this IServiceCollection serviceCollection,
        ServiceLifetime contextLifetime,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContextImplementation : DbContext, TContextService
        where TContextService : class, IReadOnlyDbContext
        => AddReadOnlyDbContext<IReadOnlyDbContext, TContextImplementation>(
            serviceCollection,
            (Action<IServiceProvider, DbContextOptionsBuilder>?)null,
            contextLifetime,
            optionsLifetime);

    /// <summary>
    ///     <para>
    ///         Registers the given context as a service in the <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure a connection string and other options.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same <see cref="DbContext" />
    ///         instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same <see cref="DbContext" />
    ///         instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-di">Using DbContext with dependency injection</see> for more information.
    ///     </para>
    ///     <para>
    ///         This overload has an <paramref name="optionsAction" /> that provides the application's
    ///         <see cref="IServiceProvider" />. This is useful if you want to setup Entity Framework Core to resolve
    ///         its internal services from the primary application service provider.
    ///         By default, we recommend using
    ///         <see cref="AddDbContext{TContext}(IServiceCollection,Action{DbContextOptionsBuilder},ServiceLifetime,ServiceLifetime)" />
    ///         which allows Entity Framework to create and maintain its own <see cref="IServiceProvider" /> for internal
    ///         Entity Framework services.
    ///     </para>
    /// </summary>
    /// <typeparam name="TContext">The type of context to be registered.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="optionsAction">
    ///     <para>
    ///         An optional action to configure the <see cref="DbContextOptions" /> for the context. This provides an
    ///         alternative to performing configuration of the context by overriding the
    ///         <see cref="DbContext.OnConfiguring" /> method in your derived context.
    ///     </para>
    ///     <para>
    ///         If an action is supplied here, the <see cref="DbContext.OnConfiguring" /> method will still be run if it has
    ///         been overridden on the derived context. <see cref="DbContext.OnConfiguring" /> configuration will be applied
    ///         in addition to configuration performed here.
    ///     </para>
    ///     <para>
    ///         In order for the options to be passed into your context, you need to expose a constructor on your context that takes
    ///         <see cref="DbContextOptions{TContext}" /> and passes it to the base constructor of <see cref="DbContext" />.
    ///     </para>
    /// </param>
    /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
    /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddReadOnlyDbContext<TContext>(
        this IServiceCollection serviceCollection,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContext : DbContext, IReadOnlyDbContext
        => AddReadOnlyDbContext<IReadOnlyDbContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);

    /// <summary>
    ///     <para>
    ///         Registers the given context as a service in the <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure a connection string and other options.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same <see cref="DbContext" />
    ///         instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-di">Using DbContext with dependency injection</see> for more information.
    ///     </para>
    ///     <para>
    ///         This overload has an <paramref name="optionsAction" /> that provides the application's
    ///         <see cref="IServiceProvider" />. This is useful if you want to setup Entity Framework Core to resolve
    ///         its internal services from the primary application service provider.
    ///         By default, we recommend using
    ///         <see
    ///             cref="AddDbContext{TContext,TContextImplementation}(IServiceCollection,Action{DbContextOptionsBuilder},ServiceLifetime,ServiceLifetime)" />
    ///         which allows Entity Framework to create and maintain its own <see cref="IServiceProvider" /> for internal
    ///         Entity Framework services.
    ///     </para>
    /// </summary>
    /// <typeparam name="TContextService">The class or interface that will be used to resolve the context from the container.</typeparam>
    /// <typeparam name="TContextImplementation">The concrete implementation type to create.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="optionsAction">
    ///     <para>
    ///         An optional action to configure the <see cref="DbContextOptions" /> for the context. This provides an
    ///         alternative to performing configuration of the context by overriding the
    ///         <see cref="DbContext.OnConfiguring" /> method in your derived context.
    ///     </para>
    ///     <para>
    ///         If an action is supplied here, the <see cref="DbContext.OnConfiguring" /> method will still be run if it has
    ///         been overridden on the derived context. <see cref="DbContext.OnConfiguring" /> configuration will be applied
    ///         in addition to configuration performed here.
    ///     </para>
    ///     <para>
    ///         In order for the options to be passed into your context, you need to expose a constructor on your context that takes
    ///         <see cref="DbContextOptions{TContext}" /> and passes it to the base constructor of <see cref="DbContext" />.
    ///     </para>
    /// </param>
    /// <param name="contextLifetime">The lifetime with which to register the DbContext service in the container.</param>
    /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddReadOnlyDbContext<TContextService, TContextImplementation>(
        this IServiceCollection serviceCollection,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContextImplementation : DbContext, TContextService
        where TContextService : IReadOnlyDbContext
        => serviceCollection.AddDbContext<TContextService, TContextImplementation>(optionsAction, contextLifetime, optionsLifetime);
    #endregion



}
