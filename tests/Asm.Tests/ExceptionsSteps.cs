using Asm.Testing;

namespace Asm.Tests;

[Binding]
public class ExceptionsSteps(ScenarioContext context)
{
    private AsmException _asmException;
    private NotFoundException _notFoundException;
    private ExistsException _existsException;
    private NotAuthorisedException _notAuthorisedException;
    private Exception _innerException;

    [Given(@"I have an inner exception with message '(.*)'")]
    public void GivenIHaveAnInnerExceptionWithMessage(string message)
    {
        _innerException = new Exception(message);
    }

    // AsmException steps
    [When(@"I create an AsmException with the default constructor")]
    public void WhenICreateAnAsmExceptionWithTheDefaultConstructor()
    {
        _asmException = new AsmException();
    }

    [When(@"I create an AsmException with message '(.*)'")]
    public void WhenICreateAnAsmExceptionWithMessage(string message)
    {
        _asmException = new AsmException(message);
    }

    [When(@"I create an AsmException with message '(.*)' and the inner exception")]
    public void WhenICreateAnAsmExceptionWithMessageAndInnerException(string message)
    {
        _asmException = new AsmException(message, _innerException!);
    }

    [When(@"I create an AsmException with error id (.*)")]
    public void WhenICreateAnAsmExceptionWithErrorId(int errorId)
    {
        _asmException = new AsmException(errorId);
    }

    [When(@"I create an AsmException with message '(.*)' and error id (.*)")]
    public void WhenICreateAnAsmExceptionWithMessageAndErrorId(string message, int errorId)
    {
        _asmException = new AsmException(message, errorId);
    }

    [When(@"I create an AsmException with message '(.*)', error id (.*) and the inner exception")]
    public void WhenICreateAnAsmExceptionWithMessageErrorIdAndInnerException(string message, int errorId)
    {
        _asmException = new AsmException(message, errorId, _innerException!);
    }

    [Then(@"the AsmException has a unique Id")]
    public void ThenTheAsmExceptionHasAUniqueId()
    {
        Assert.NotEqual(Guid.Empty, _asmException!.Id);
    }

    [Then(@"the AsmException ErrorId is (.*)")]
    public void ThenTheAsmExceptionErrorIdIs(int errorId)
    {
        Assert.Equal(errorId, _asmException!.ErrorId);
    }

    [Then(@"the AsmException message is '(.*)'")]
    public void ThenTheAsmExceptionMessageIs(string message)
    {
        Assert.Equal(message, _asmException!.Message);
    }

    [Then(@"the AsmException has an inner exception with message '(.*)'")]
    public void ThenTheAsmExceptionHasAnInnerExceptionWithMessage(string message)
    {
        Assert.NotNull(_asmException!.InnerException);
        Assert.Equal(message, _asmException.InnerException!.Message);
    }

    // NotFoundException steps
    [When(@"I create a NotFoundException with the default constructor")]
    public void WhenICreateANotFoundExceptionWithTheDefaultConstructor()
    {
        _notFoundException = new NotFoundException();
    }

    [When(@"I create a NotFoundException with message '(.*)'")]
    public void WhenICreateANotFoundExceptionWithMessage(string message)
    {
        _notFoundException = new NotFoundException(message);
    }

    [When(@"I create a NotFoundException with message '(.*)' and the inner exception")]
    public void WhenICreateANotFoundExceptionWithMessageAndInnerException(string message)
    {
        _notFoundException = new NotFoundException(message, _innerException!);
    }

    [Then(@"the NotFoundException message is '(.*)'")]
    public void ThenTheNotFoundExceptionMessageIs(string message)
    {
        Assert.Equal(message, _notFoundException!.Message);
    }

    [Then(@"the NotFoundException has an inner exception with message '(.*)'")]
    public void ThenTheNotFoundExceptionHasAnInnerExceptionWithMessage(string message)
    {
        Assert.NotNull(_notFoundException!.InnerException);
        Assert.Equal(message, _notFoundException.InnerException!.Message);
    }

    [When(@"I throw a NotFoundException with message '(.*)'")]
    public void WhenIThrowANotFoundExceptionWithMessage(string message)
    {
        context.CatchException(() => throw new NotFoundException(message));
    }

    // ExistsException steps
    [When(@"I create an ExistsException with the default constructor")]
    public void WhenICreateAnExistsExceptionWithTheDefaultConstructor()
    {
        _existsException = new ExistsException();
    }

    [When(@"I create an ExistsException with message '(.*)'")]
    public void WhenICreateAnExistsExceptionWithMessage(string message)
    {
        _existsException = new ExistsException(message);
    }

    [When(@"I create an ExistsException with message '(.*)' and the inner exception")]
    public void WhenICreateAnExistsExceptionWithMessageAndInnerException(string message)
    {
        _existsException = new ExistsException(message, _innerException!);
    }

    [Then(@"the ExistsException message is '(.*)'")]
    public void ThenTheExistsExceptionMessageIs(string message)
    {
        Assert.Equal(message, _existsException!.Message);
    }

    [Then(@"the ExistsException has an inner exception with message '(.*)'")]
    public void ThenTheExistsExceptionHasAnInnerExceptionWithMessage(string message)
    {
        Assert.NotNull(_existsException!.InnerException);
        Assert.Equal(message, _existsException.InnerException!.Message);
    }

    [When(@"I throw an ExistsException with message '(.*)'")]
    public void WhenIThrowAnExistsExceptionWithMessage(string message)
    {
        context.CatchException(() => throw new ExistsException(message));
    }

    // NotAuthorisedException steps
    [When(@"I create a NotAuthorisedException with the default constructor")]
    public void WhenICreateANotAuthorisedExceptionWithTheDefaultConstructor()
    {
        _notAuthorisedException = new NotAuthorisedException();
    }

    [When(@"I create a NotAuthorisedException with message '([^']+)'$")]
    public void WhenICreateANotAuthorisedExceptionWithMessage(string message)
    {
        _notAuthorisedException = new NotAuthorisedException(message);
    }

    [When(@"I create a NotAuthorisedException with message '(.*)' and the inner exception")]
    public void WhenICreateANotAuthorisedExceptionWithMessageAndInnerException(string message)
    {
        _notAuthorisedException = new NotAuthorisedException(message, _innerException!);
    }

    [When(@"I create a NotAuthorisedException with message '([^']+)' and type '([^']+)'")]
    public void WhenICreateANotAuthorisedExceptionWithMessageAndType(string message, string typeName)
    {
        var type = Type.GetType(typeName)!;
        _notAuthorisedException = new NotAuthorisedException(message, type);
    }

    [When(@"I create a NotAuthorisedException with message '([^']+)', type '([^']+)' and state '([^']+)'")]
    public void WhenICreateANotAuthorisedExceptionWithMessageTypeAndState(string message, string typeName, string state)
    {
        var type = Type.GetType(typeName)!;
        _notAuthorisedException = new NotAuthorisedException(message, type, state);
    }

    [Then(@"the NotAuthorisedException message is '(.*)'")]
    public void ThenTheNotAuthorisedExceptionMessageIs(string message)
    {
        Assert.Equal(message, _notAuthorisedException!.Message);
    }

    [Then(@"the NotAuthorisedException has an inner exception with message '(.*)'")]
    public void ThenTheNotAuthorisedExceptionHasAnInnerExceptionWithMessage(string message)
    {
        Assert.NotNull(_notAuthorisedException!.InnerException);
        Assert.Equal(message, _notAuthorisedException.InnerException!.Message);
    }

    [When(@"I throw a NotAuthorisedException with message '(.*)'")]
    public void WhenIThrowANotAuthorisedExceptionWithMessage(string message)
    {
        context.CatchException(() => throw new NotAuthorisedException(message));
    }
}
