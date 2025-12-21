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
}