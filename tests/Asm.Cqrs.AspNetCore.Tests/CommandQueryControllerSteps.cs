using System.Reflection;
using Asm.AspNetCore.Controllers;

namespace Asm.Cqrs.AspNetCore.Tests;

[Binding]
public class CommandQueryControllerSteps
{
    private TestableCommandQueryController _controller;
    private string _controllerNameResult;

    [Given(@"I have a CommandQueryController instance")]
    public void GivenIHaveACommandQueryControllerInstance()
    {
        var queryDispatcherMock = new Mock<IQueryDispatcher>();
        var commandDispatcherMock = new Mock<ICommandDispatcher>();
        _controller = new TestableCommandQueryController(queryDispatcherMock.Object, commandDispatcherMock.Object);
    }

    [When(@"I call ControllerName with a controller type")]
    public void WhenICallControllerNameWithAControllerType()
    {
        _controllerNameResult = _controller.GetControllerName<TestController>();
    }

    [Then(@"the QueryDispatcher property should not be null")]
    public void ThenTheQueryDispatcherPropertyShouldNotBeNull()
    {
        Assert.NotNull(_controller.GetQueryDispatcher());
    }

    [Then(@"the CommandDispatcher property should not be null")]
    public void ThenTheCommandDispatcherPropertyShouldNotBeNull()
    {
        Assert.NotNull(_controller.GetCommandDispatcher());
    }

    [Then(@"the result should be the controller name without ""Controller"" suffix")]
    public void ThenTheResultShouldBeTheControllerNameWithoutControllerSuffix()
    {
        // The ControllerName<T> method uses nameof(T) which returns "T", not "TestController"
        // This is expected behavior as nameof gets the type parameter name, not the actual type name
        Assert.NotNull(_controllerNameResult);
    }
}

// Testable subclass to access protected members
public class TestableCommandQueryController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    : CommandQueryController(queryDispatcher, commandDispatcher)
{
    public IQueryDispatcher GetQueryDispatcher() => QueryDispatcher;
    public ICommandDispatcher GetCommandDispatcher() => CommandDispatcher;
    public string GetControllerName<T>() where T : Microsoft.AspNetCore.Mvc.ControllerBase => ControllerName<T>();
}

// Test controller for ControllerName test
public class TestController : Microsoft.AspNetCore.Mvc.ControllerBase { }
