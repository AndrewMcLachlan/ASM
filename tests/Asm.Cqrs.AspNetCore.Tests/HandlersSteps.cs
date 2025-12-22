using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Asm.Cqrs.AspNetCore.Tests;

[Binding]
public class HandlersSteps
{
    private Mock<IQueryDispatcher> _queryDispatcherMock;
    private Mock<ICommandDispatcher> _commandDispatcherMock;
    private DefaultHttpContext _httpContext;
    private IResult _result;
    private int _intResult;
    private bool _commandDispatched;
    private object _expectedResult;
    private Delegate _lastHandler;
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
            .Setup(x => x.Dispatch(It.IsAny<ICommand>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask)
            .Callback(() => _commandDispatched = true);
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

    [When(@"I invoke HandleQuery with the query")]
    public async Task WhenIInvokeHandleQueryWithTheQuery()
    {
        var handleQueryMethod = typeof(Handlers).GetMethod("HandleQuery", BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(TestQuery), typeof(string));

        var query = new TestQuery("test");
        var resultTask = (ValueTask<IResult>)handleQueryMethod.Invoke(null, [query, _queryDispatcherMock.Object, CancellationToken.None])!;
        _result = await resultTask;
    }

    [When(@"I invoke HandlePagedQuery with the query")]
    public async Task WhenIInvokeHandlePagedQueryWithTheQuery()
    {
        var handlePagedQueryMethod = typeof(Handlers).GetMethod("HandlePagedQuery", BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(TestPagedQuery), typeof(string));

        var query = new TestPagedQuery();
        var resultTask = (ValueTask<IResult>)handlePagedQueryMethod.Invoke(null, [query, _httpContext, _queryDispatcherMock.Object, CancellationToken.None])!;
        _result = await resultTask;
    }

    [When(@"I invoke HandleDelete with a delete command")]
    public async Task WhenIInvokeHandleDeleteWithADeleteCommand()
    {
        var handleDeleteMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "HandleDelete" && m.GetGenericArguments().Length == 1)
            .MakeGenericMethod(typeof(TestDeleteCommand));

        var command = new TestDeleteCommand(1);
        var resultTask = (ValueTask<IResult>)handleDeleteMethod.Invoke(null, [command, _commandDispatcherMock.Object, CancellationToken.None])!;
        _result = await resultTask;
    }

    [When(@"I invoke HandleDelete with response with the command")]
    public async Task WhenIInvokeHandleDeleteWithResponseWithTheCommand()
    {
        var handleDeleteMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "HandleDelete" && m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(typeof(TestDeleteCommandWithResponse), typeof(string));

        var command = new TestDeleteCommandWithResponse(1);
        var resultTask = (ValueTask<IResult>)handleDeleteMethod.Invoke(null, [command, _commandDispatcherMock.Object, CancellationToken.None])!;
        _result = await resultTask;
    }

    [When(@"I invoke HandleCommand with the command")]
    public async Task WhenIInvokeHandleCommandWithTheCommand()
    {
        var handleCommandMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "HandleCommand" && m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(typeof(TestCommand), typeof(int));

        var command = new TestCommand("test");
        var resultTask = (ValueTask<int>)handleCommandMethod.Invoke(null, [command, _commandDispatcherMock.Object, CancellationToken.None])!;
        _intResult = await resultTask;
    }

    [When(@"I invoke HandleCommand without response")]
    public async Task WhenIInvokeHandleCommandWithoutResponse()
    {
        _commandDispatched = false;
        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestCommandNoResponse>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask)
            .Callback(() => _commandDispatched = true);

        var handleCommandMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "HandleCommand" && m.GetGenericArguments().Length == 1)
            .MakeGenericMethod(typeof(TestCommandNoResponse));

        var command = new TestCommandNoResponse("test");
        var resultTask = (ValueTask)handleCommandMethod.Invoke(null, [command, _commandDispatcherMock.Object, CancellationToken.None])!;
        await resultTask;
    }

    [When(@"I invoke CreateCreateHandler with route name '([^']+)' and the command$")]
    public async Task WhenIInvokeCreateCreateHandlerWithRouteNameAndTheCommand(string routeName)
    {
        var createCreateHandlerMethod = typeof(Handlers).GetMethod("CreateCreateHandler", BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(TestCreateCommand), typeof(TestCreatedItem));

        Func<TestCreatedItem, object> getRouteParams = item => new { id = item.Id };
        var handler = (Delegate)createCreateHandlerMethod.Invoke(null, [routeName, getRouteParams, CommandBinding.None])!;

        var command = new TestCreateCommand("Test");
        var task = (Task<IResult>)handler.DynamicInvoke(command, _commandDispatcherMock.Object, CancellationToken.None)!;
        _result = await task;
    }

    [When(@"I invoke CreateCommandHandler with status code (.*)")]
    public async Task WhenIInvokeCreateCommandHandlerWithStatusCode(int statusCode)
    {
        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestCommandNoResponse>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var createCommandHandlerMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "CreateCommandHandler" && m.GetGenericArguments().Length == 1)
            .MakeGenericMethod(typeof(TestCommandNoResponse));

        var handler = (Delegate)createCommandHandlerMethod.Invoke(null, [statusCode, CommandBinding.None])!;

        var command = new TestCommandNoResponse("Test");
        var task = (Task<IResult>)handler.DynamicInvoke(command, _commandDispatcherMock.Object, CancellationToken.None)!;
        _result = await task;
    }

    [When(@"I invoke CreateCommandHandler with response and status code (.*)")]
    public async Task WhenIInvokeCreateCommandHandlerWithResponseAndStatusCode(int statusCode)
    {
        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var createCommandHandlerMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "CreateCommandHandler" && m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(typeof(TestCommand), typeof(int));

        var handler = (Delegate)createCommandHandlerMethod.Invoke(null, [statusCode, CommandBinding.None])!;

        var command = new TestCommand("Test");
        var task = (Task<IResult>)handler.DynamicInvoke(command, _commandDispatcherMock.Object, CancellationToken.None)!;
        _result = await task;
    }

    [When(@"I invoke CreateCommandHandler with status code (.*) and binding '(.*)'")]
    public async Task WhenIInvokeCreateCommandHandlerWithStatusCodeAndBinding(int statusCode, string bindingName)
    {
        var binding = Enum.Parse<CommandBinding>(bindingName);
        _lastBinding = binding;

        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestCommandNoResponse>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var createCommandHandlerMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "CreateCommandHandler" && m.GetGenericArguments().Length == 1)
            .MakeGenericMethod(typeof(TestCommandNoResponse));

        _lastHandler = (Delegate)createCommandHandlerMethod.Invoke(null, [statusCode, binding])!;

        var command = new TestCommandNoResponse("Test");
        var task = (Task<IResult>)_lastHandler.DynamicInvoke(command, _commandDispatcherMock.Object, CancellationToken.None)!;
        _result = await task;
    }

    [When(@"I invoke CreateCommandHandler with response, status code (.*) and binding '(.*)'")]
    public async Task WhenIInvokeCreateCommandHandlerWithResponseStatusCodeAndBinding(int statusCode, string bindingName)
    {
        var binding = Enum.Parse<CommandBinding>(bindingName);
        _lastBinding = binding;

        _commandDispatcherMock
            .Setup(x => x.Dispatch(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var createCommandHandlerMethod = typeof(Handlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.Name == "CreateCommandHandler" && m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(typeof(TestCommand), typeof(int));

        _lastHandler = (Delegate)createCommandHandlerMethod.Invoke(null, [statusCode, binding])!;

        var command = new TestCommand("Test");
        var task = (Task<IResult>)_lastHandler.DynamicInvoke(command, _commandDispatcherMock.Object, CancellationToken.None)!;
        _result = await task;
    }

    [When(@"I invoke CreateCreateHandler with route name '(.*)', binding '(.*)' and the command")]
    public async Task WhenIInvokeCreateCreateHandlerWithRouteNameBindingAndTheCommand(string routeName, string bindingName)
    {
        var binding = Enum.Parse<CommandBinding>(bindingName);
        _lastBinding = binding;

        var createCreateHandlerMethod = typeof(Handlers).GetMethod("CreateCreateHandler", BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(TestCreateCommand), typeof(TestCreatedItem));

        Func<TestCreatedItem, object> getRouteParams = item => new { id = item.Id };
        _lastHandler = (Delegate)createCreateHandlerMethod.Invoke(null, [routeName, getRouteParams, binding])!;

        var command = new TestCreateCommand("Test");
        var task = (Task<IResult>)_lastHandler.DynamicInvoke(command, _commandDispatcherMock.Object, CancellationToken.None)!;
        _result = await task;
    }

    [Then(@"the result should be Ok with value '(.*)'")]
    public void ThenTheResultShouldBeOkWithValue(string expectedValue)
    {
        var okResult = Assert.IsType<Ok<string>>(_result);
        Assert.Equal(expectedValue, okResult.Value);
    }

    [Then(@"the result should be Ok")]
    public void ThenTheResultShouldBeOk()
    {
        Assert.IsAssignableFrom<IStatusCodeHttpResult>(_result);
        var statusResult = (IStatusCodeHttpResult)_result;
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

    [Then(@"the command result should be (.*)")]
    public void ThenTheCommandResultShouldBe(int expectedValue)
    {
        Assert.Equal(expectedValue, _intResult);
    }

    [Then(@"the command should be dispatched")]
    public void ThenTheCommandShouldBeDispatched()
    {
        Assert.True(_commandDispatched);
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

        // Verify the delegate has the expected parameter attributes based on binding
        var method = _lastHandler.Method;
        var parameters = method.GetParameters();

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
                    // None binding should not have either attribute on the outer lambda
                    break;
            }
        }
    }
}

// Helper class to access internal Handlers
internal static class Handlers
{
    internal static ValueTask<IResult> HandleQuery<TQuery, TResult>(TQuery query, IQueryDispatcher dispatcher, CancellationToken cancellationToken) where TQuery : IQuery<TResult>
        => InvokeInternal<ValueTask<IResult>>("HandleQuery", [typeof(TQuery), typeof(TResult)], query, dispatcher, cancellationToken);

    internal static ValueTask<IResult> HandlePagedQuery<TQuery, TResult>(TQuery query, HttpContext http, IQueryDispatcher dispatcher, CancellationToken cancellationToken) where TQuery : IQuery<PagedResult<TResult>>
        => InvokeInternal<ValueTask<IResult>>("HandlePagedQuery", [typeof(TQuery), typeof(TResult)], query, http, dispatcher, cancellationToken);

    internal static ValueTask<IResult> HandleDelete<TRequest>(TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand
        => InvokeInternal<ValueTask<IResult>>("HandleDelete", [typeof(TRequest)], request, dispatcher, cancellationToken);

    internal static ValueTask<IResult> HandleDelete<TRequest, TResult>(TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand<TResult>
        => InvokeInternal<ValueTask<IResult>>("HandleDelete", [typeof(TRequest), typeof(TResult)], request, dispatcher, cancellationToken);

    internal static ValueTask<TResult> HandleCommand<TRequest, TResult>(TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand<TResult>
        => InvokeInternal<ValueTask<TResult>>("HandleCommand", [typeof(TRequest), typeof(TResult)], request, dispatcher, cancellationToken);

    internal static ValueTask HandleCommand<TRequest>(TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand
        => InvokeInternal<ValueTask>("HandleCommand", [typeof(TRequest)], request, dispatcher, cancellationToken);

    internal static Delegate CreateCreateHandler<TRequest, TResult>(string routeName, Func<TResult, object> getRouteParams, CommandBinding binding = CommandBinding.None) where TRequest : ICommand<TResult>
        => InvokeInternal<Delegate>("CreateCreateHandler", [typeof(TRequest), typeof(TResult)], routeName, getRouteParams, binding);

    internal static Delegate CreateCommandHandler<TRequest>(int returnStatusCode, CommandBinding binding = CommandBinding.None) where TRequest : ICommand
        => InvokeInternal<Delegate>("CreateCommandHandler", [typeof(TRequest)], returnStatusCode, binding);

    internal static Delegate CreateCommandHandler<TRequest, TResponse>(int returnStatusCode, CommandBinding binding = CommandBinding.None) where TRequest : ICommand<TResponse>
        => InvokeInternal<Delegate>("CreateCommandHandler", [typeof(TRequest), typeof(TResponse)], returnStatusCode, binding);

    private static T InvokeInternal<T>(string methodName, Type[] genericTypes, params object[] args)
    {
        var handlersType = typeof(Asm.AspNetCore.IEndpointRouteBuilderExtensions).Assembly.GetType("Asm.AspNetCore.Handlers")!;
        var methods = handlersType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(m => m.Name == methodName && m.GetGenericArguments().Length == genericTypes.Length);

        var method = methods.First();
        var genericMethod = method.MakeGenericMethod(genericTypes);
        return (T)genericMethod.Invoke(null, args)!;
    }
}
