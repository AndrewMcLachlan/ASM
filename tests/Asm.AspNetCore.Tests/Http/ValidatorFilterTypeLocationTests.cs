using Asm.AspNetCore.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Tests.Http;

public class ValidatorFilterTypeLocationTests
{
    public sealed class Model
    {
        public string Name { get; init; } = "";
    }

    private sealed class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator() => RuleFor(m => m.Name).NotEmpty();
    }

    private static EndpointFilterInvocationContext CreateContext(params object?[] arguments)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<Model>, ModelValidator>();

        var httpContext = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
        return new DefaultEndpointFilterInvocationContext(httpContext, arguments);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task InvokeAsync_LocatesArgumentByType_RegardlessOfPosition()
    {
        // The Model is the second argument; the type-based filter must still find it.
        var context = CreateContext("some-string", new Model { Name = "valid" });
        var filter = new ValidatorFilter<Model>();
        var nextCalled = false;

        await filter.InvokeAsync(context, _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>("ok");
        });

        Assert.True(nextCalled);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task InvokeAsync_InvalidArgumentLocatedByType_Throws()
    {
        var context = CreateContext(42, new Model { Name = "" });
        var filter = new ValidatorFilter<Model>();

        await Assert.ThrowsAsync<ValidationException>(async () =>
            await filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>("ok")));
    }
}
