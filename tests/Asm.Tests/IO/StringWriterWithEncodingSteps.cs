using System.Text;
using Asm.IO;

namespace Asm.Tests.IO;

[Binding]
public class StringWriterWithEncodingSteps
{
    private StringWriterWithEncoding _writer = null!;

    [Given(@"I create a StringWriterWithEncoding with UTF8 encoding")]
    public void GivenICreateAStringWriterWithEncodingWithUtf8Encoding()
    {
        _writer = new StringWriterWithEncoding(Encoding.UTF8);
    }

    [Given(@"I create a StringWriterWithEncoding with ASCII encoding")]
    public void GivenICreateAStringWriterWithEncodingWithAsciiEncoding()
    {
        _writer = new StringWriterWithEncoding(Encoding.ASCII);
    }

    [When(@"I write '(.*)' to the writer")]
    public void WhenIWriteToTheWriter(string text)
    {
        _writer.Write(text);
    }

    [Then(@"the Encoding property should be UTF8")]
    public void ThenTheEncodingPropertyShouldBeUtf8()
    {
        Assert.Equal(Encoding.UTF8, _writer.Encoding);
    }

    [Then(@"the Encoding property should be ASCII")]
    public void ThenTheEncodingPropertyShouldBeAscii()
    {
        Assert.Equal(Encoding.ASCII, _writer.Encoding);
    }

    [Then(@"the written content should be '(.*)'")]
    public void ThenTheWrittenContentShouldBe(string expected)
    {
        Assert.Equal(expected, _writer.ToString());
    }
}
