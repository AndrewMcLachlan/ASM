using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Infrastructure;

[Binding]
public class ProblemDetailsFactorySteps
{
    private Mock<IHostEnvironment> _hostEnvironmentMock;
    private ProblemDetailsFactory _factory;
    private DefaultHttpContext _httpContext;
    private ProblemDetails _problemDetails;
    private ValidationProblemDetails _validationProblemDetails;
    private ModelStateDictionary _modelStateDictionary;

    [Given(@"I have a ProblemDetailsFactory with development environment")]
    public void GivenIHaveAProblemDetailsFactoryWithDevelopmentEnvironment()
    {
        _hostEnvironmentMock = new Mock<IHostEnvironment>();
        _hostEnvironmentMock.Setup(x => x.EnvironmentName).Returns("Development");
        _factory = new ProblemDetailsFactory(_hostEnvironmentMock.Object);
    }

    [Given(@"I have a ProblemDetailsFactory with production environment")]
    public void GivenIHaveAProblemDetailsFactoryWithProductionEnvironment()
    {
        _hostEnvironmentMock = new Mock<IHostEnvironment>();
        _hostEnvironmentMock.Setup(x => x.EnvironmentName).Returns("Production");
        _factory = new ProblemDetailsFactory(_hostEnvironmentMock.Object);
    }

    [Given(@"I have an HttpContext with no error")]
    public void GivenIHaveAnHttpContextWithNoError()
    {
        _httpContext = new DefaultHttpContext();
    }

    [Given(@"I have an HttpContext with a NotFoundException '(.*)'")]
    public void GivenIHaveAnHttpContextWithANotFoundException(string message)
    {
        _httpContext = CreateHttpContextWithException(new NotFoundException(message));
    }

    [Given(@"I have an HttpContext with an ExistsException '(.*)'")]
    public void GivenIHaveAnHttpContextWithAnExistsException(string message)
    {
        _httpContext = CreateHttpContextWithException(new ExistsException(message));
    }

    [Given(@"I have an HttpContext with a NotAuthorisedException '(.*)'")]
    public void GivenIHaveAnHttpContextWithANotAuthorisedException(string message)
    {
        _httpContext = CreateHttpContextWithException(new NotAuthorisedException(message));
    }

    [Given(@"I have an HttpContext with a BadHttpRequestException '(.*)'")]
    public void GivenIHaveAnHttpContextWithABadHttpRequestException(string message)
    {
        _httpContext = CreateHttpContextWithException(new BadHttpRequestException(message));
    }

    [Given(@"I have an HttpContext with an InvalidOperationException '(.*)'")]
    public void GivenIHaveAnHttpContextWithAnInvalidOperationException(string message)
    {
        _httpContext = CreateHttpContextWithException(new InvalidOperationException(message));
    }

    [Given(@"I have an HttpContext with an AsmException '(.*)' and error id (.*)")]
    public void GivenIHaveAnHttpContextWithAnAsmExceptionAndErrorId(string message, int errorId)
    {
        _httpContext = CreateHttpContextWithException(new AsmException(message, errorId));
    }

    [Given(@"I have an HttpContext with an unknown exception '(.*)'")]
    public void GivenIHaveAnHttpContextWithAnUnknownException(string message)
    {
        _httpContext = CreateHttpContextWithException(new Exception(message));
    }

    [Given(@"I have an HttpContext with a FluentValidation ValidationException for field '(.*)'")]
    public void GivenIHaveAnHttpContextWithAFluentValidationValidationExceptionForField(string field)
    {
        var failures = new List<ValidationFailure>
        {
            new(field, $"{field} is invalid")
        };
        var exception = new ValidationException(failures);
        _httpContext = CreateHttpContextWithException(exception);
    }

    [Given(@"I have a ModelStateDictionary with error '(.*)' '(.*)'")]
    public void GivenIHaveAModelStateDictionaryWithError(string key, string error)
    {
        _modelStateDictionary = new ModelStateDictionary();
        _modelStateDictionary.AddModelError(key, error);
    }

    [When(@"I create problem details with status (.*) and title '(.*)'")]
    public void WhenICreateProblemDetailsWithStatusAndTitle(int status, string title)
    {
        _problemDetails = _factory.CreateProblemDetails(_httpContext, status, title);
    }

    [When(@"I create problem details")]
    public void WhenICreateProblemDetails()
    {
        _problemDetails = _factory.CreateProblemDetails(_httpContext);
    }

    [When(@"I create validation problem details")]
    public void WhenICreateValidationProblemDetails()
    {
        _validationProblemDetails = _factory.CreateValidationProblemDetails(_httpContext, _modelStateDictionary);
    }

    [When(@"I create validation problem details with title '(.*)'")]
    public void WhenICreateValidationProblemDetailsWithTitle(string title)
    {
        _validationProblemDetails = _factory.CreateValidationProblemDetails(_httpContext, _modelStateDictionary, title: title);
    }

    [Then(@"the problem details should have status (.*)")]
    public void ThenTheProblemDetailsShouldHaveStatus(int expectedStatus)
    {
        Assert.Equal(expectedStatus, _problemDetails.Status);
    }

    [Then(@"the problem details should have title '(.*)'")]
    public void ThenTheProblemDetailsShouldHaveTitle(string expectedTitle)
    {
        Assert.Equal(expectedTitle, _problemDetails.Title);
    }

    [Then(@"the problem details should have detail '(.*)'")]
    public void ThenTheProblemDetailsShouldHaveDetail(string expectedDetail)
    {
        Assert.Equal(expectedDetail, _problemDetails.Detail);
    }

    [Then(@"the problem details should have extension '(.*)' with value (.*)")]
    public void ThenTheProblemDetailsShouldHaveExtensionWithValue(string key, int value)
    {
        Assert.True(_problemDetails.Extensions.ContainsKey(key));
        Assert.Equal(value, _problemDetails.Extensions[key]);
    }

    [Then(@"the problem details should contain exception details")]
    public void ThenTheProblemDetailsShouldContainExceptionDetails()
    {
        Assert.NotNull(_problemDetails.Detail);
        Assert.Contains("Exception", _problemDetails.Detail);
    }

    [Then(@"the problem details should not contain exception details")]
    public void ThenTheProblemDetailsShouldNotContainExceptionDetails()
    {
        Assert.Null(_problemDetails.Detail);
    }

    [Then(@"the validation problem details should have status (.*)")]
    public void ThenTheValidationProblemDetailsShouldHaveStatus(int expectedStatus)
    {
        Assert.Equal(expectedStatus, _validationProblemDetails.Status);
    }

    [Then(@"the validation problem details should contain error for '(.*)'")]
    public void ThenTheValidationProblemDetailsShouldContainErrorFor(string key)
    {
        Assert.True(_validationProblemDetails.Errors.ContainsKey(key));
    }

    [Then(@"the validation problem details should have title '(.*)'")]
    public void ThenTheValidationProblemDetailsShouldHaveTitle(string expectedTitle)
    {
        Assert.Equal(expectedTitle, _validationProblemDetails.Title);
    }

    private static DefaultHttpContext CreateHttpContextWithException(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var exceptionHandlerFeature = new Mock<IExceptionHandlerFeature>();
        exceptionHandlerFeature.Setup(x => x.Error).Returns(exception);
        httpContext.Features.Set(exceptionHandlerFeature.Object);
        return httpContext;
    }
}
