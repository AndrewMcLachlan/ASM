using Asm.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Cqrs.Tests.Queries;

public class QueryTests
{
    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Description", "Tests a Generic Query")]
    public async Task TestGenericQuery()
    {
        ServiceCollection services = new();

        services.AddQueryHandlers(GetType().Assembly);

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        var result = await queryDispatcher.Dispatch(new TestQuery { Input = "Abc" }, TestContext.Current.CancellationToken);

        Assert.Equal("ABC", result);
    }
}