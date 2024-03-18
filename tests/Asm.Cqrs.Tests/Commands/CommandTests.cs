using Asm.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Cqrs.Tests.Commands;

public class CommandTests
{
    [Fact]
    public async Task NegaTest()
    {
        ServiceCollection services = new();

        services.AddCommandHandlers(GetType().Assembly);

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await commandDispatcher.Dispatch(new TestCommand { Input = "Abc" });

        Assert.False(result);
    }

    [Fact]
    public async Task PosiTest()
    {
        ServiceCollection services = new();

        services.AddCommandHandlers(GetType().Assembly);

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await commandDispatcher.Dispatch(new TestCommand { Input = "ABC" });

        Assert.True(result);
    }
}