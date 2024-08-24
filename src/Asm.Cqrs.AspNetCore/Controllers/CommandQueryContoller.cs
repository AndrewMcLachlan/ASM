using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asm.AspNetCore.Controllers;

/// <summary>
/// A controller that supports CQRS commands and queries.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CommandQueryController"/> class.
/// </remarks>
/// <param name="queryDispatcher">A query dispatcher instance.</param>
/// <param name="commandDispatcher">A command dispatcher instance.</param>
[ApiController]
[Authorize]
public abstract class CommandQueryController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher) : ControllerBase
{
    /// <summary>
    /// A dispatcher for queries.
    /// </summary>
    protected IQueryDispatcher QueryDispatcher { get; private set; } = queryDispatcher;

    /// <summary>
    /// A dispatcher for commands.
    /// </summary>
    protected ICommandDispatcher CommandDispatcher { get; private set; } = commandDispatcher;

    /// <summary>
    /// Gets the name of a controller with the "Controller" suffix removed.
    /// </summary>
    /// <typeparam name="T">The controller instance.</typeparam>
    /// <returns>The name of the controller.</returns>
    protected string ControllerName<T>() where T : ControllerBase =>
        nameof(T).Replace("Controller", String.Empty);
}
