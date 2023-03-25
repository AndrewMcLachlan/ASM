using Asm.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Cqrs.Tests.Commands
{
    public class CommandTests
    {
        [Fact]
        public void NegaTest()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddCommandHandlers(GetType().Assembly);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            var result = commandDispatcher.Dispatch(new TestCommand { Input = "Abc" }).Result;

            Assert.False(result);
        }

        [Fact]
        public void PosiTest()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddCommandHandlers(GetType().Assembly);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            var result = commandDispatcher.Dispatch(new TestCommand { Input = "ABC" }).Result;

            Assert.True(result);
        }
    }
}