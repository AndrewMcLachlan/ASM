using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asm.AspNetCore.Controllers;

/// <summary>
/// A controller that supports CQRS commands and queries.
/// </summary>
[ApiController]
[Authorize]
public abstract class CommandQueryController : ControllerBase
{
    /// <summary>
    /// A dispatcher for queries.
    /// </summary>
    protected IQueryDispatcher QueryDispatcher { get; private set; }

    /// <summary>
    /// A dispatcher for commands.
    /// </summary>
    protected ICommandDispatcher CommandDispatcher { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandQueryController"/> class.
    /// </summary>
    /// <param name="queryDispatcher">A query dispatcher instance.</param>
    /// <param name="commandDispatcher">A command dispatcher instance.</param>
    public CommandQueryController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    {
        QueryDispatcher = queryDispatcher;
        CommandDispatcher = commandDispatcher;
    }

    /// <summary>
    /// Gets the name of a controller with the "Controller" suffix removed.
    /// </summary>
    /// <typeparam name="T">The controller instance.</typeparam>
    /// <returns>The name of the controller.</returns>
    protected string ControllerName<T>() where T : ControllerBase =>
        nameof(T).Replace("Controller", String.Empty);
}
