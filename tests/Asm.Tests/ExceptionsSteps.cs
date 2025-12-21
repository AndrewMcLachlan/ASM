namespace Asm.Tests;

[Binding]
public class ExceptionsSteps
{
    private AsmException _asmException = null!;
    private Exception _innerException = null!;

    [Given(@"I have an inner exception with message '(.*)'")]
    public void GivenIHaveAnInnerExceptionWithMessage(string message)
    {
        _innerException = new Exception(message);
    }

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
        _asmException = new AsmException(message, _innerException);
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
        _asmException = new AsmException(message, errorId, _innerException);
    }

    [Then(@"the AsmException has a unique Id")]
    public void ThenTheAsmExceptionHasAUniqueId()
    {
        Assert.NotEqual(Guid.Empty, _asmException.Id);
    }

    [Then(@"the AsmException ErrorId is (.*)")]
    public void ThenTheAsmExceptionErrorIdIs(int errorId)
    {
        Assert.Equal(errorId, _asmException.ErrorId);
    }

    [Then(@"the AsmException message is '(.*)'")]
    public void ThenTheAsmExceptionMessageIs(string message)
    {
        Assert.Equal(message, _asmException.Message);
    }

    [Then(@"the AsmException has an inner exception with message '(.*)'")]
    public void ThenTheAsmExceptionHasAnInnerExceptionWithMessage(string message)
    {
        Assert.NotNull(_asmException.InnerException);
        Assert.Equal(message, _asmException.InnerException.Message);
    }
}
