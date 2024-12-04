using System.Text;
using System.Text.RegularExpressions;

namespace Asm.Win32.Tests;

public partial class HostsTests
{
    private readonly static string HostsFileInternal;
    private readonly static string BadHostsFile;

    public static TheoryData<string> HostsFile => [HostsFileInternal];

    static HostsTests()
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

        HostsFileInternal = sb.ToString();

        sb.AppendLine("Wibble");

        BadHostsFile = sb.ToString();
    }


    [Fact]
    [Trait("Category", "Win32")]
    public void HostsLoadTest1()
    {
        Assert.True(Hosts.Current.Entries.Count > 0);
    }

    [Theory]
    [MemberData(nameof(HostsFile))]
    [Trait("Category", "Win32")]
    public void HostsLoadTest2(string hostsFile)
    {
        Hosts hosts;
        using (MemoryStream stream = new(Encoding.UTF8.GetBytes(hostsFile)))
        {
            hosts = new Hosts(stream);
        }

        Assert.Equal(8, hosts.Entries.Count);

        Assert.Equal(HostEntryType.Comment, hosts.Entries[0].EntryType);
        Assert.Equal(HostEntryType.Blank, hosts.Entries[1].EntryType);
        Assert.Equal(HostEntryType.Comment, hosts.Entries[2].EntryType);
        Assert.Equal(HostEntryType.Blank, hosts.Entries[3].EntryType);
        Assert.Equal(HostEntryType.IPv4Entry, hosts.Entries[4].EntryType);
        Assert.Equal(HostEntryType.IPv4Entry, hosts.Entries[5].EntryType);
        Assert.Equal(HostEntryType.IPv4Entry, hosts.Entries[6].EntryType);
        Assert.Equal(HostEntryType.IPv4EntryCommented, hosts.Entries[7].EntryType);

        Assert.Equal("##Comment###", hosts.Entries[0].Comment);
        Assert.Equal(" Comment with space", hosts.Entries[2].Comment);
        Assert.Equal("127.0.0.1", hosts.Entries[4].Address.ToString());
        Assert.Equal("localhost", hosts.Entries[4].Alias.ToString());
        Assert.Equal("127.0.0.1", hosts.Entries[5].Address.ToString());
        Assert.Equal("dave", hosts.Entries[5].Alias.ToString());
        Assert.Equal("With comment", hosts.Entries[5].Comment);
        Assert.Equal("127.0.0.1", hosts.Entries[6].Address.ToString());
        Assert.Equal("commented", hosts.Entries[6].Alias.ToString());
        Assert.Equal("#With comment", hosts.Entries[6].Comment);
        Assert.Equal("127.0.0.1", hosts.Entries[7].Address.ToString());
        Assert.Equal("commented", hosts.Entries[7].Alias.ToString());
        Assert.Equal("#With comment", hosts.Entries[7].Comment);
        Assert.True(hosts.Entries[7].IsCommented);
    }

    [Fact]
    [Trait("Category", "Win32")]
    public void HostsLoadTest3()
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(BadHostsFile));
        Assert.Throws<FormatException>(() => new Hosts(stream));
    }

    [Fact]
    public void RegexTest()
    {
        string line = "127.0.0.1\tHello";
        Regex commentMatch = CommentRegex();
        Match match = commentMatch.Match(line);

        Assert.False(match.Success);

        _ = commentMatch.Match("###Hello");
    }

    [GeneratedRegex(@"^[#]{1}?(.*)$")]
    private static partial Regex CommentRegex();
}
