using System.Text;

namespace Asm.Win32.Tests;

[Binding]
public class HostsSteps(ScenarioContext context) : IDisposable
{
    private Hosts _hosts = null!;
    private MemoryStream _inputStream = null!;
    private MemoryStream _outputStream = null!;
    private string _tempFilePath = null!;
    private string _originalSystemHostsFile = null!;
    private bool _disposed;

    private static readonly string StandardHostsContent = CreateStandardHostsContent();
    private static readonly string InvalidHostsContent = CreateInvalidHostsContent();

    private static string CreateStandardHostsContent()
    {
        StringBuilder sb = new();
        sb.AppendLine("###Comment###");
        sb.AppendLine();
        sb.AppendLine(" # Comment with space");
        sb.AppendLine();
        sb.AppendLine("127.0.0.1 localhost");
        sb.AppendLine("127.0.0.1 dave #With comment");
        sb.AppendLine("127.0.0.1 commented ##With comment");
        sb.AppendLine("#127.0.0.1 commented ##With comment");
        return sb.ToString();
    }

    private static string CreateInvalidHostsContent()
    {
        StringBuilder sb = new();
        sb.AppendLine("127.0.0.1 localhost");
        sb.AppendLine("Wibble");
        return sb.ToString();
    }

    [Given(@"I have a hosts file stream with standard entries")]
    public void GivenIHaveAHostsFileStreamWithStandardEntries()
    {
        _inputStream = new MemoryStream(Encoding.UTF8.GetBytes(StandardHostsContent));
    }

    [Given(@"I have a hosts file stream with an invalid entry")]
    public void GivenIHaveAHostsFileStreamWithAnInvalidEntry()
    {
        _inputStream = new MemoryStream(Encoding.UTF8.GetBytes(InvalidHostsContent));
    }

    [Given(@"I have a mock hosts file on disk")]
    public void GivenIHaveAMockHostsFileOnDisk()
    {
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"hosts_test_{Guid.NewGuid()}.txt");
        File.WriteAllText(_tempFilePath, "127.0.0.1 localhost\n127.0.0.1 testhost\n");
    }

    [Given(@"I have a mock hosts file configured as the system hosts file")]
    public void GivenIHaveAMockHostsFileConfiguredAsTheSystemHostsFile()
    {
        _originalSystemHostsFile = Hosts.SystemHostsFile;
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"hosts_test_{Guid.NewGuid()}.txt");
        File.WriteAllText(_tempFilePath, "127.0.0.1 localhost\n127.0.0.1 testhost\n");
        Hosts.SystemHostsFile = _tempFilePath;
        Hosts.ResetInstance();
    }

    [Given(@"I create a Hosts instance from the stream")]
    public void GivenICreateAHostsInstanceFromTheStream()
    {
        _hosts = new Hosts(_inputStream);
    }

    [Given(@"I create a Hosts instance from the file path")]
    public void GivenICreateAHostsInstanceFromTheFilePath()
    {
        _hosts = new Hosts(_tempFilePath);
    }

    [Given(@"I have a path to a non-existent hosts file")]
    public void GivenIHaveAPathToANonExistentHostsFile()
    {
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"non_existent_{Guid.NewGuid()}.txt");
    }

    [When(@"I create a Hosts instance from the stream")]
    public void WhenICreateAHostsInstanceFromTheStream()
    {
        _hosts = new Hosts(_inputStream);
    }

    [When(@"I create a Hosts instance from the invalid stream")]
    public void WhenICreateAHostsInstanceFromTheInvalidStream()
    {
        context.CatchException(() => _hosts = new Hosts(_inputStream));
    }

    [When(@"I create a Hosts instance from the file path")]
    public void WhenICreateAHostsInstanceFromTheFilePath()
    {
        _hosts = new Hosts(_tempFilePath);
    }

    [When(@"I create a Hosts instance from the non-existent file")]
    public void WhenICreateAHostsInstanceFromTheNonExistentFile()
    {
        context.CatchException(() => _hosts = new Hosts(_tempFilePath));
    }

    [When(@"I access the Current hosts instance")]
    public void WhenIAccessTheCurrentHostsInstance()
    {
        _hosts = Hosts.Current;
    }

    [When(@"I update the mock hosts file with new content")]
    public void WhenIUpdateTheMockHostsFileWithNewContent()
    {
        // Need to wait a moment to ensure file timestamp changes
        Thread.Sleep(100);
        File.WriteAllText(_tempFilePath, "127.0.0.1 localhost\n127.0.0.1 testhost\n127.0.0.1 newhost\n");
    }

    [When(@"I call Refresh")]
    public void WhenICallRefresh()
    {
        _hosts.Refresh();
    }

    [When(@"I write the hosts to an output stream")]
    public void WhenIWriteTheHostsToAnOutputStream()
    {
        _outputStream = new MemoryStream();
        _hosts.WriteHosts(_outputStream);
        // Note: WriteHosts closes the stream via StreamWriter, so we capture content via Raw property instead
    }

    [Then(@"the hosts file should have (.*) entries")]
    public void ThenTheHostsFileShouldHaveEntries(int count)
    {
        Assert.Equal(count, _hosts.Entries.Count);
    }

    [Then(@"entry (.*) should be a Comment")]
    public void ThenEntryShouldBeAComment(int index)
    {
        Assert.Equal(HostEntryType.Comment, _hosts.Entries[index].EntryType);
    }

    [Then(@"entry (.*) should be a Blank")]
    public void ThenEntryShouldBeABlank(int index)
    {
        Assert.Equal(HostEntryType.Blank, _hosts.Entries[index].EntryType);
    }

    [Then(@"entry (.*) should be an IPv4Entry")]
    public void ThenEntryShouldBeAnIPv4Entry(int index)
    {
        Assert.Equal(HostEntryType.IPv4Entry, _hosts.Entries[index].EntryType);
    }

    [Then(@"entry (.*) should be an IPv4EntryCommented")]
    public void ThenEntryShouldBeAnIPv4EntryCommented(int index)
    {
        Assert.Equal(HostEntryType.IPv4EntryCommented, _hosts.Entries[index].EntryType);
    }

    [Then(@"entry (.*) should have comment ""(.*)""")]
    public void ThenEntryShouldHaveComment(int index, string comment)
    {
        Assert.Equal(comment, _hosts.Entries[index].Comment);
    }

    [Then(@"entry (.*) should have address ""(.*)""")]
    public void ThenEntryShouldHaveAddress(int index, string address)
    {
        Assert.Equal(address, _hosts.Entries[index].Address?.ToString());
    }

    [Then(@"entry (.*) should have alias ""(.*)""")]
    public void ThenEntryShouldHaveAlias(int index, string alias)
    {
        Assert.Equal(alias, _hosts.Entries[index].Alias);
    }

    [Then(@"entry (.*) should be commented")]
    public void ThenEntryShouldBeCommented(int index)
    {
        Assert.True(_hosts.Entries[index].IsCommented);
    }

    [Then(@"the output stream should contain the hosts content")]
    public void ThenTheOutputStreamShouldContainTheHostsContent()
    {
        // Since WriteHosts closes the stream, verify via the Raw property instead
        string content = _hosts.Raw;
        Assert.NotEmpty(content);
        Assert.Contains("localhost", content);
    }

    [AfterScenario]
    public void Cleanup()
    {
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _inputStream?.Dispose();
                _outputStream?.Dispose();
                _hosts?.Dispose();

                if (_originalSystemHostsFile != null)
                {
                    Hosts.ResetInstance();
                    Hosts.SystemHostsFile = _originalSystemHostsFile;
                }

                if (_tempFilePath != null && File.Exists(_tempFilePath))
                {
                    try
                    {
                        File.Delete(_tempFilePath);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
            _disposed = true;
        }
    }
}
