using Asm.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Cqrs.Tests.Queries
{
    public class QueryTests
    {
        [Fact]
        public void Test()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddQueryHandlers(GetType().Assembly);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

            var result = queryDispatcher.Dispatch(new TestQuery { Input = "Abc" }).Result;

            Assert.Equal("ABC", result);
        }
    }
}