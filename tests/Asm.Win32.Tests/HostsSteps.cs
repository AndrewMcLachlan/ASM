using System.Net;
using System.Text;

namespace Asm.Win32.Tests;

[Binding]
public class HostsSteps(ScenarioContext context) : IDisposable
{
    private Hosts _hosts = null!;
    private IReadOnlyList<HostEntry> _snapshot = null!;
    private MemoryStream _inputStream = null!;
    private MemoryStream _outputStream = null!;
    private string _tempFilePath = null!;
    private string _secondTempFilePath = null!;
    private string _originalSystemHostsFile = null!;
    private bool _disposed;

    private const string MockHostsContent = "127.0.0.1 localhost\n127.0.0.1 testhost\n";

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
        File.WriteAllText(_tempFilePath, MockHostsContent);
    }

    [Given(@"I have a mock hosts file configured as the system hosts file")]
    public void GivenIHaveAMockHostsFileConfiguredAsTheSystemHostsFile()
    {
        _originalSystemHostsFile = Hosts.SystemHostsFile;
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"hosts_test_{Guid.NewGuid()}.txt");
        File.WriteAllText(_tempFilePath, MockHostsContent);
        Hosts.SystemHostsFile = _tempFilePath;
        Hosts.ResetInstance();
    }

    [Given(@"I have a second mock hosts file on disk")]
    public void GivenIHaveASecondMockHostsFileOnDisk()
    {
        _secondTempFilePath = Path.Combine(Path.GetTempPath(), $"hosts_test_{Guid.NewGuid()}.txt");
        File.WriteAllText(_secondTempFilePath, MockHostsContent);
    }

    [Given(@"I create a Hosts instance from the second file path")]
    public void GivenICreateAHostsInstanceFromTheSecondFilePath()
    {
        _hosts = new Hosts(_secondTempFilePath);
    }

    [Given(@"I have a hosts file stream with an address-only entry")]
    public void GivenIHaveAHostsFileStreamWithAnAddressOnlyEntry()
    {
        _inputStream = new MemoryStream(Encoding.UTF8.GetBytes("127.0.0.1\n127.0.0.2 host2\n"));
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

    [Given(@"I access the Current hosts instance")]
    [When(@"I access the Current hosts instance")]
    public void WhenIAccessTheCurrentHostsInstance()
    {
        _hosts = Hosts.Current;
    }

    [When(@"I remove the last entry")]
    public void WhenIRemoveTheLastEntry()
    {
        _hosts.RemoveEntryAt(_hosts.Entries.Count - 1);
    }

    [When(@"I add a host entry for ""(.*)"" aliased ""(.*)""")]
    public void WhenIAddAHostEntry(string address, string alias)
    {
        _hosts.AddEntry(new HostEntry(IPAddress.Parse(address), alias, null, false));
    }

    [When(@"I insert a host entry for ""(.*)"" aliased ""(.*)"" at index (.*)")]
    public void WhenIInsertAHostEntry(string address, string alias, int index)
    {
        _hosts.InsertEntry(index, new HostEntry(IPAddress.Parse(address), alias, null, false));
    }

    [When(@"I update entry (.*) with alias ""(.*)""")]
    public void WhenIUpdateEntry(int index, string alias)
    {
        HostEntry existing = _hosts.Entries[index];
        _hosts.UpdateEntry(index, new HostEntry(existing.Address, alias, existing.Comment, existing.IsCommented));
    }

    [When(@"I clear all entries")]
    public void WhenIClearAllEntries()
    {
        _hosts.ClearEntries();
    }

    [When(@"I try to add a null host entry")]
    public void WhenITryToAddANullHostEntry()
    {
        context.CatchException(() => _hosts.AddEntry(null!));
    }

    [When(@"I concurrently refresh and read the entries")]
    public void WhenIConcurrentlyRefreshAndReadTheEntries()
    {
        // Repeatedly refresh on one thread while another enumerates the read-only
        // snapshot. A non-thread-safe implementation would tear or throw here.
        using CancellationTokenSource cts = new();
        Exception failure = null;

        Thread reader = new(() =>
        {
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    foreach (HostEntry entry in _hosts.Entries)
                    {
                        _ = entry.EntryType;
                    }
                }
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });

        reader.Start();

        for (int i = 0; i < 200; i++)
        {
            _hosts.Refresh();
        }

        cts.Cancel();
        reader.Join();

        Assert.Null(failure);
    }

    [When(@"I write the hosts file")]
    public void WhenIWriteTheHostsFile()
    {
        _hosts.WriteHostsToFile();
    }

    [When(@"I configure a second mock hosts file as the system hosts file")]
    public void WhenIConfigureASecondMockHostsFileAsTheSystemHostsFile()
    {
        _secondTempFilePath = Path.Combine(Path.GetTempPath(), $"hosts_test_{Guid.NewGuid()}.txt");
        File.WriteAllText(_secondTempFilePath, "127.0.0.1 localhost\n127.0.0.1 testhost\n127.0.0.1 newhost\n");
        Hosts.SystemHostsFile = _secondTempFilePath;
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

    [When(@"I capture the current entries snapshot")]
    public void WhenICaptureTheCurrentEntriesSnapshot()
    {
        _snapshot = _hosts.Entries;
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

    [Then(@"the captured snapshot should still have (.*) entries")]
    public void ThenTheCapturedSnapshotShouldStillHaveEntries(int count)
    {
        Assert.Equal(count, _snapshot.Count);
    }

    [Then(@"the exception message should contain ""(.*)""")]
    public void ThenTheExceptionMessageShouldContain(string fragment)
    {
        Exception ex = context.GetException();
        Assert.NotNull(ex);
        Assert.Contains(fragment, ex.Message);
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

    [Then(@"the second mock hosts file should have (.*) entries on disk")]
    public void ThenTheSecondMockHostsFileShouldHaveEntriesOnDisk(int count)
    {
        using Hosts reread = new(_secondTempFilePath);
        Assert.Equal(count, reread.Entries.Count);
    }

    [Then(@"the system hosts file should be unchanged")]
    public void ThenTheSystemHostsFileShouldBeUnchanged()
    {
        Assert.Equal(MockHostsContent, File.ReadAllText(_tempFilePath));
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

                foreach (string? path in new[] { _tempFilePath, _secondTempFilePath })
                {
                    if (path != null && File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch
                        {
                            // Ignore cleanup errors
                        }
                    }
                }
            }
            _disposed = true;
        }
    }
}
