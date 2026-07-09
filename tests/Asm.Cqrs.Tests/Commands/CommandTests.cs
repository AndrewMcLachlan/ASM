using Asm.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Cqrs.Tests.Commands;

public class CommandTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task NegaTest()
    {
        ServiceCollection services = new();

        services.AddCommandHandlers(GetType().Assembly);

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await commandDispatcher.Dispatch(new TestCommand { Input = "Abc" }, TestContext.Current.CancellationToken);

        Assert.False(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task PosiTest()
    {
        ServiceCollection services = new();

        services.AddCommandHandlers(GetType().Assembly);

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await commandDispatcher.Dispatch(new TestCommand { Input = "ABC" }, TestContext.Current.CancellationToken);

        Assert.True(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SynchronousHandlerThrow_SurfacesOriginalExceptionType()
    {
        ServiceCollection services = new();
        services.AddCommandHandlers(GetType().Assembly);
        var commandDispatcher = services.BuildServiceProvider().GetRequiredService<ICommandDispatcher>();

        // A handler that throws synchronously must surface its own exception type, not a
        // TargetInvocationException from the reflection dispatch.
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await commandDispatcher.Dispatch(new ThrowingCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ResponseCommand_DispatchedAsVoidCommand_ThrowsHelpfulError()
    {
        ServiceCollection services = new();
        services.AddCommandHandlers(GetType().Assembly);
        var commandDispatcher = services.BuildServiceProvider().GetRequiredService<ICommandDispatcher>();

        // TestCommand implements ICommand<bool>; dispatching it through a variable typed as the
        // non-generic ICommand selects the void overload, which cannot resolve a handler.
        ICommand command = new TestCommand { Input = "ABC" };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await commandDispatcher.Dispatch(command, TestContext.Current.CancellationToken));

        Assert.Contains("Dispatch<TResponse>", exception.Message);
    }
}