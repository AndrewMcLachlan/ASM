using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Asm.Cqrs.AspNetCore.Tests;

[Binding]
public class HandlersSteps
{
    private Mock<IQueryDispatcher> _queryDispatcherMock = null!;
    private Mock<ICommandDispatcher> _commandDispatcherMock = null!;
    private DefaultHttpContext _httpContext = null!;
    private IResult _result = null!;
    private bool _commandExecuted;
    private object _expectedResult = null!;
    private Delegate _lastHandler = null!;
    private CommandBinding _lastBinding;

    // Test Query/Command types
    private record TestQuery(string Value) : IQuery<string>;
    private record TestPagedQuery() : IQuery<PagedResult<string>>;
    private record TestDeleteCommand(int Id) : ICommand;
    private record TestDeleteCommandWithResponse(int Id) : ICommand<string>;
    private record TestCommand(string Value) : ICommand<int>;
    private record TestCommandNoResponse(string Value) : ICommand;
    private record TestCreateCommand(string Name) : ICommand<TestCreatedItem>;
    private record TestCreatedItem(int Id, string Name);

    private static readonly Type ProductionHandlers =
        typeof(Asm.AspNetCore.AsmCqrsAspNetCoreEndpointRouteBuilderExtensions).Assembly.GetType("Asm.AspNetCore.Handlers")!;

    private static MethodInfo Factory(string name, int genericArgs, Func<MethodInfo, bool>? filter = null) =>
        ProductionHandlers.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(m => m.Name == name && m.GetGenericArguments().Length == genericArgs)
            .First(m => filter?.Invoke(m) ?? true);

    // ── Given ───────────────────────────────────────────────────────────────

    [Given(@"I have a query dispatcher that returns '(.*)'")]
    public void GivenIHaveAQueryDispatcherThatReturns(string result)
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        _queryDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        _expectedResult = result;
    }

    [Given(@"I have a query dispatcher that returns a paged result with (.*) total items")]
    public void GivenIHaveAQueryDispatcherThatReturnsAPagedResultWithTotalItems(int totalItems)
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        var pagedResult = new PagedResult<string> { Results = ["item1", "item2"], Total = totalItems };
        _queryDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestPagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);
    }

    [Given(@"I have an HttpContext")]
    public void GivenIHaveAnHttpContext()
    {
        _httpContext = new DefaultHttpContext();
    }

    [Given(@"I have a command dispatcher")]
    public void GivenIHaveACommandDispatcher()
    {
        _commandDispatcherMock = new Mock<ICommandDispatcher>();
        _commandDispatcherMock
            .Setup(x => x.Execute(It.IsAny<ICommand>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask)
            .Callback(() => _commandExecuted = true);
    }

    [Given(@"I have a command dispatcher that returns '(.*)'")]
    public void GivenIHaveACommandDispatcherThatReturnsString(string result)
    {
        _commandDispatcherMock = new Mock<ICommandDispatcher>();
        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestDeleteCommandWithResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        _expectedResult = result;
    }

    [Given(@"I have a command dispatcher that returns (.*)")]
    public void GivenIHaveACommandDispatcherThatReturnsInt(int result)
    {
        _commandDispatcherMock = new Mock<ICommandDispatcher>();
        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
    }

    [Given(@"I have a command dispatcher that returns a created item with id (.*)")]
    public void GivenIHaveACommandDispatcherThatReturnsACreatedItemWithId(int id)
    {
        _commandDispatcherMock = new Mock<ICommandDispatcher>();
        var createdItem = new TestCreatedItem(id, "Test Item");
        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestCreateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdItem);
        _expectedResult = createdItem;
    }

    // ── When ────────────────────────────────────────────────────────────────

    [When(@"I invoke the query handler")]
    public async Task WhenIInvokeTheQueryHandler()
    {
        var method = Factory("HandleQuery", 2).MakeGenericMethod(typeof(TestQuery), typeof(string));
        _result = await (ValueTask<IResult>)method.Invoke(null, [new TestQuery("test"), _queryDispatcherMock.Object, CancellationToken.None])!;
    }

    [When(@"I invoke the paged query handler")]
    public async Task WhenIInvokeThePagedQueryHandler()
    {
        var method = Factory("HandlePagedQuery", 2).MakeGenericMethod(typeof(TestPagedQuery), typeof(string));
        _result = await (ValueTask<IResult>)method.Invoke(null, [new TestPagedQuery(), _httpContext, _queryDispatcherMock.Object, CancellationToken.None])!;
    }

    [When(@"I invoke the delete handler")]
    public Task WhenIInvokeTheDeleteHandler() => InvokeDeleteHandler(CommandBinding.None);

    [When(@"I invoke the delete handler with response")]
    public async Task WhenIInvokeTheDeleteHandlerWithResponse()
    {
        var handler = (Delegate)Factory("CreateCommandHandler", 2)
            .MakeGenericMethod(typeof(TestDeleteCommandWithResponse), typeof(string))
            .Invoke(null, [StatusCodes.Status200OK, CommandBinding.None])!;
        _result = await (Task<IResult>)handler.DynamicInvoke(new TestDeleteCommandWithResponse(1), _commandDispatcherMock.Object, CancellationToken.None)!;
    }

    [When(@"I invoke the command handler with response")]
    public Task WhenIInvokeTheCommandHandlerWithResponse() => InvokeValueCommandHandler(StatusCodes.Status200OK, CommandBinding.None);

    [When(@"I invoke the command handler with response and status code (.*)")]
    public Task WhenIInvokeTheCommandHandlerWithResponseAndStatusCode(int statusCode) => InvokeValueCommandHandler(statusCode, CommandBinding.None);

    [When(@"I invoke the command handler with response and binding '(.*)'")]
    public Task WhenIInvokeTheCommandHandlerWithResponseAndBinding(string binding) => InvokeValueCommandHandler(StatusCodes.Status200OK, Enum.Parse<CommandBinding>(binding));

    [When(@"I invoke the void command handler")]
    public Task WhenIInvokeTheVoidCommandHandler() => InvokeVoidCommandHandler(StatusCodes.Status204NoContent, CommandBinding.None);

    [When(@"I invoke the void command handler with status code (.*)")]
    public Task WhenIInvokeTheVoidCommandHandlerWithStatusCode(int statusCode) => InvokeVoidCommandHandler(statusCode, CommandBinding.None);

    [When(@"I invoke the void command handler with binding '(.*)'")]
    public Task WhenIInvokeTheVoidCommandHandlerWithBinding(string binding) => InvokeVoidCommandHandler(StatusCodes.Status204NoContent, Enum.Parse<CommandBinding>(binding));

    [When(@"I invoke the create handler with route name '([^']+)'$")]
    public Task WhenIInvokeTheCreateHandler(string routeName) => InvokeCreateHandler(routeName, CommandBinding.None);

    [When(@"I invoke the create handler with route name '(.*)' and binding '(.*)'")]
    public Task WhenIInvokeTheCreateHandlerWithBinding(string routeName, string binding) => InvokeCreateHandler(routeName, Enum.Parse<CommandBinding>(binding));

    private async Task InvokeDeleteHandler(CommandBinding binding)
    {
        _lastBinding = binding;
        _lastHandler = (Delegate)Factory("CreateDeleteHandler", 1).MakeGenericMethod(typeof(TestDeleteCommand)).Invoke(null, [binding])!;
        _result = await (Task<IResult>)_lastHandler.DynamicInvoke(new TestDeleteCommand(1), _commandDispatcherMock.Object, CancellationToken.None)!;
    }

    private async Task InvokeValueCommandHandler(int statusCode, CommandBinding binding)
    {
        _lastBinding = binding;
        _lastHandler = (Delegate)Factory("CreateCommandHandler", 2).MakeGenericMethod(typeof(TestCommand), typeof(int)).Invoke(null, [statusCode, binding])!;
        _result = await (Task<IResult>)_lastHandler.DynamicInvoke(new TestCommand("Test"), _commandDispatcherMock.Object, CancellationToken.None)!;
    }

    private async Task InvokeVoidCommandHandler(int statusCode, CommandBinding binding)
    {
        _commandExecuted = false;
        _lastBinding = binding;
        _lastHandler = (Delegate)Factory("CreateVoidCommandHandler", 1).MakeGenericMethod(typeof(TestCommandNoResponse)).Invoke(null, [statusCode, binding])!;
        _result = await (Task<IResult>)_lastHandler.DynamicInvoke(new TestCommandNoResponse("Test"), _commandDispatcherMock.Object, CancellationToken.None)!;
    }

    private async Task InvokeCreateHandler(string routeName, CommandBinding binding)
    {
        _lastBinding = binding;
        var factory = Factory("CreateCreateHandler", 2, m => m.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2)
            .MakeGenericMethod(typeof(TestCreateCommand), typeof(TestCreatedItem));
        Func<TestCreatedItem, object> getRouteParams = item => new { id = item.Id };
        _lastHandler = (Delegate)factory.Invoke(null, [routeName, getRouteParams, binding])!;
        _result = await (Task<IResult>)_lastHandler.DynamicInvoke(new TestCreateCommand("Test"), _commandDispatcherMock.Object, CancellationToken.None)!;
    }

    // ── Then ────────────────────────────────────────────────────────────────

    [Then(@"the result should be Ok with value '(.*)'")]
    public void ThenTheResultShouldBeOkWithValue(string expectedValue)
    {
        var okResult = Assert.IsType<Ok<string>>(_result);
        Assert.Equal(expectedValue, okResult.Value);
    }

    [Then(@"the result should be Ok with int value (.*)")]
    public void ThenTheResultShouldBeOkWithIntValue(int expectedValue)
    {
        var okResult = Assert.IsType<Ok<int>>(_result);
        Assert.Equal(expectedValue, okResult.Value);
    }

    [Then(@"the result should be Ok")]
    public void ThenTheResultShouldBeOk()
    {
        var statusResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(_result);
        Assert.Equal(StatusCodes.Status200OK, statusResult.StatusCode);
    }

    [Then(@"the X-Total-Count header should be '(.*)'")]
    public void ThenTheXTotalCountHeaderShouldBe(string expectedValue)
    {
        Assert.True(_httpContext.Response.Headers.ContainsKey("X-Total-Count"));
        Assert.Equal(expectedValue, _httpContext.Response.Headers["X-Total-Count"].ToString());
    }

    [Then(@"the result should be NoContent")]
    public void ThenTheResultShouldBeNoContent()
    {
        Assert.IsType<NoContent>(_result);
    }

    [Then(@"the command should be executed")]
    public void ThenTheCommandShouldBeExecuted()
    {
        Assert.True(_commandExecuted);
    }

    [Then(@"the result should be CreatedAtRoute")]
    public void ThenTheResultShouldBeCreatedAtRoute()
    {
        Assert.IsType<CreatedAtRoute<TestCreatedItem>>(_result);
    }

    [Then(@"the result should have status code (.*)")]
    public void ThenTheResultShouldHaveStatusCode(int expectedStatusCode)
    {
        var statusResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(_result);
        Assert.Equal(expectedStatusCode, statusResult.StatusCode);
    }

    [Then(@"the handler should use '(.*)' binding")]
    public void ThenTheHandlerShouldUseBinding(string bindingName)
    {
        var expectedBinding = Enum.Parse<CommandBinding>(bindingName);
        Assert.Equal(expectedBinding, _lastBinding);

        // Verify the delegate has the expected parameter attributes based on binding.
        var parameters = _lastHandler.Method.GetParameters();

        if (parameters.Length > 0)
        {
            var firstParam = parameters[0];
            var hasFromBody = firstParam.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.FromBodyAttribute), false).Length > 0;
            var hasAsParameters = firstParam.GetCustomAttributes(typeof(Microsoft.AspNetCore.Http.AsParametersAttribute), false).Length > 0;

            switch (expectedBinding)
            {
                case CommandBinding.Body:
                    Assert.True(hasFromBody, "Expected [FromBody] attribute on first parameter");
                    break;
                case CommandBinding.Parameters:
                    Assert.True(hasAsParameters, "Expected [AsParameters] attribute on first parameter");
                    break;
                case CommandBinding.None:
                    // None binding should not have either attribute on the outer lambda.
                    break;
            }
        }
    }
}
