using System.IO;
using System.Text;

namespace Asm.Tests.Extensions;

[Binding]
public class StreamExtensionsSteps(ScenarioContext context)
{
    private MemoryStream? _stream;
    private byte[] _writeBuffer = [];
    private byte[] _readBuffer = [];

    [Given(@"I have a MemoryStream with data ""(.*)""")]
    public void GivenIHaveAMemoryStreamWithData(string data)
    {
        _stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
    }

    [Given(@"I have an empty MemoryStream")]
    public void GivenIHaveAnEmptyMemoryStream()
    {
        _stream = new MemoryStream();
    }

    [Given(@"I have a read buffer of size (\d+)")]
    public void GivenIHaveAReadBufferOfSize(int size)
    {
        _readBuffer = new byte[size];
    }

    [Given(@"I have a write buffer with data ""(.*)""")]
    public void GivenIHaveAWriteBufferWithData(string data)
    {
        _writeBuffer = Encoding.UTF8.GetBytes(data);
    }

    [Given(@"I have a null stream")]
    public void GivenIHaveANullStream()
    {
        _stream = null;
    }

    [When(@"I read (\d+) bytes into buffer offset (\d+) using long parameters")]
    public void WhenIReadBytesIntoBufferOffsetUsingLongParameters(long count, long offset)
    {
        _stream!.Read(_readBuffer, offset, count);
    }

    [When(@"I write (\d+) bytes from buffer offset (\d+) using long parameters")]
    public void WhenIWriteBytesFromBufferOffsetUsingLongParameters(long count, long offset)
    {
        _stream!.Write(_writeBuffer, offset, count);
    }

    [When(@"I try to read from the null stream")]
    public void WhenITryToReadFromTheNullStream()
    {
        context.CatchException(() =>
        {
            var buffer = new byte[10];
            _stream!.Read(buffer, 0L, 10L);
        });
    }

    [When(@"I try to write to the null stream")]
    public void WhenITryToWriteToTheNullStream()
    {
        context.CatchException(() =>
        {
            var buffer = new byte[10];
            _stream!.Write(buffer, 0L, 10L);
        });
    }

    [Then(@"the read buffer should contain ""(.*)""")]
    public void ThenTheReadBufferShouldContain(string expected)
    {
        var actual = Encoding.UTF8.GetString(_readBuffer);
        Assert.Equal(expected, actual);
    }

    [Then(@"the read buffer at offset (\d+) with length (\d+) should be ""(.*)""")]
    public void ThenTheReadBufferAtOffsetWithLengthShouldBe(int offset, int length, string expected)
    {
        var actual = Encoding.UTF8.GetString(_readBuffer, offset, length);
        Assert.Equal(expected, actual);
    }

    [Then(@"the stream should contain ""(.*)""")]
    public void ThenTheStreamShouldContain(string expected)
    {
        _stream!.Position = 0;
        using var reader = new StreamReader(_stream!);
        var actual = reader.ReadToEnd();
        Assert.Equal(expected, actual);
    }
}
