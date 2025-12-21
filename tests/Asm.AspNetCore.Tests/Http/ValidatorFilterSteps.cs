#nullable enable

using Asm.AspNetCore.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Tests.Http;

[Binding]
public class ValidatorFilterSteps
{
    private ValidatorFilter<TestRequest> _filter = null!;
    private EndpointFilterInvocationContext _context = null!;
    private EndpointFilterDelegate _next = null!;
    private object? _result;
    private Exception? _exception;
    private bool _nextCalled;

    [Given(@"I have a valid test request")]
    public void GivenIHaveAValidTestRequest()
    {
        SetupFilter(new TestRequest { Name = "ValidName", Email = "valid@example.com" });
    }

    [Given(@"I have an invalid test request")]
    public void GivenIHaveAnInvalidTestRequest()
    {
        SetupFilter(new TestRequest { Name = "", Email = "invalid-email" });
    }

    private void SetupFilter(TestRequest request)
    {
        _filter = new ValidatorFilter<TestRequest>(0);

        var services = new ServiceCollection();
        services.AddSingleton<IValidator<TestRequest>, TestRequestValidator>();
        var serviceProvider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        _context = new DefaultEndpointFilterInvocationContext(httpContext, request);

        _nextCalled = false;
        _next = (context) =>
        {
            _nextCalled = true;
            return ValueTask.FromResult<object?>("Success");
        };
    }

    [When(@"the validator filter is invoked")]
    public async Task WhenTheValidatorFilterIsInvoked()
    {
        try
        {
            _result = await _filter.InvokeAsync(_context, _next);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the next delegate should be called")]
    public void ThenTheNextDelegateShouldBeCalled()
    {
        Assert.True(_nextCalled, "Expected next delegate to be called");
    }

    [Then(@"the result should be the expected response")]
    public void ThenTheResultShouldBeTheExpectedResponse()
    {
        Assert.Equal("Success", _result);
    }

    [Then(@"a validation exception should be thrown")]
    public void ThenAValidationExceptionShouldBeThrown()
    {
        Assert.NotNull(_exception);
        Assert.IsType<ValidationException>(_exception);
    }

    public class TestRequest
    {
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
    }

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid");
        }
    }
}
