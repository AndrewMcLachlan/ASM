using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asm.AspNetCore.Controllers;

[ApiController]
[Authorize]
public abstract class CommandQueryController : ControllerBase
{
    protected IQueryDispatcher QueryDispatcher { get; private set; }
    protected ICommandDispatcher CommandDispatcher { get; private set; }

    public CommandQueryController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    {
        QueryDispatcher = queryDispatcher;
        CommandDispatcher = commandDispatcher;
    }

    protected string ControllerName<T>() where T : ControllerBase =>
        nameof(T).Replace("Controller", String.Empty);
}
