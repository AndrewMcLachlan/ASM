namespace Asm.Tests;

[Binding]
public class ExceptionsSteps
{
    // AsmException is abstract; exercise its behaviour through a minimal concrete subclass.
    private sealed class TestAsmException : AsmException
    {
        public TestAsmException() : base() { }
        public TestAsmException(string message) : base(message) { }
        public TestAsmException(string message, Exception innerException) : base(message, innerException) { }
        public TestAsmException(int errorId) : base(errorId) { }
        public TestAsmException(string message, int errorId) : base(message, errorId) { }
        public TestAsmException(string message, int errorId, Exception innerException) : base(message, errorId, innerException) { }
    }

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
        _asmException = new TestAsmException();
    }

    [When(@"I create an AsmException with message '(.*)'")]
    public void WhenICreateAnAsmExceptionWithMessage(string message)
    {
        _asmException = new TestAsmException(message);
    }

    [When(@"I create an AsmException with message '(.*)' and the inner exception")]
    public void WhenICreateAnAsmExceptionWithMessageAndInnerException(string message)
    {
        _asmException = new TestAsmException(message, _innerException);
    }

    [When(@"I create an AsmException with error id (.*)")]
    public void WhenICreateAnAsmExceptionWithErrorId(int errorId)
    {
        _asmException = new TestAsmException(errorId);
    }

    [When(@"I create an AsmException with message '(.*)' and error id (.*)")]
    public void WhenICreateAnAsmExceptionWithMessageAndErrorId(string message, int errorId)
    {
        _asmException = new TestAsmException(message, errorId);
    }

    [When(@"I create an AsmException with message '(.*)', error id (.*) and the inner exception")]
    public void WhenICreateAnAsmExceptionWithMessageErrorIdAndInnerException(string message, int errorId)
    {
        _asmException = new TestAsmException(message, errorId, _innerException);
    }

    [Then(@"AsmException is abstract")]
    public void ThenAsmExceptionIsAbstract()
    {
        Assert.True(typeof(AsmException).IsAbstract);
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
