using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Asm.Win32;

/// <summary>
/// Class for controlling the Windows Hosts file.
/// </summary>
public sealed class Hosts : IDisposable
{
    #region Constants
    private static readonly string HostsFile = Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\drivers\etc\hosts";
    #endregion

    #region Events
    /// <summary>
    /// Fired when the Windows hosts file has been changed.
    /// </summary>
    public event EventHandler? HostsFileChanged;
    #endregion

    #region Fields
    private static readonly Hosts _instance = new();
    private readonly string? _hostsFile;
    private readonly Timer? _pollTimer;
    private DateTime _hostsFileLastUpdated;
    private bool _disposed;
    #endregion

    #region Properties
    /// <summary>
    /// The current Windows hosts file.
    /// </summary>
    public static Hosts Current { get { return _instance; } }

    /// <summary>
    /// The raw content of the hosts file.
    /// </summary>
    public string Raw
    {
        get
        {
            return GetRaw();
        }
    }

    /// <summary>
    /// The host file entries.
    /// </summary>
    public IList<HostEntry> Entries
    {
        get;
        private set;
    } = new List<HostEntry>();
    #endregion

    #region Constructors
    private Hosts() : this(HostsFile)
    {
        _pollTimer = new Timer(10000);
        _pollTimer.Elapsed += new ElapsedEventHandler(PollTimer_Elapsed);
        _pollTimer.Start();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Hosts"/> class.
    /// </summary>
    /// <param name="file">The hosts file name.</param>
    public Hosts(string file)
    {
        _hostsFile = file;
        LoadHostsFile(file);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Hosts"/> class.
    /// </summary>
    /// <param name="hosts">The hosts file.</param>
    public Hosts(Stream hosts)
    {
        ReadHosts(hosts);
    }

    /// <summary>
    /// Finializer.
    /// </summary>
    ~Hosts()
    {
        this.Dispose(false);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Writes the host entries to the given stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public void WriteHosts(Stream stream)
    {
        using StreamWriter writer = new(stream);
        writer.Write(Raw);
    }

    /// <summary>
    /// Writes the host entries to the Windows hosts file.
    /// </summary>
    public void WriteHostsToFile()
    {
        WriteHostsToFile(HostsFile);
        OnHostsFileChanged();
    }

    /// <summary>
    /// Writes the hosts entries to given file.
    /// </summary>
    /// <param name="fileName">The name of the to write to.</param>
    public void WriteHostsToFile(string fileName)
    {
        using (FileStream stream = File.OpenWrite(fileName))
        {
            WriteHosts(stream);
        }
        _hostsFileLastUpdated = File.GetLastWriteTime(fileName);
    }

    /// <summary>
    /// Refreshes the hosts file from disk. Any unsaved changes will be lost.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no hosts file was specified when the object was created.</exception>
    public void Refresh()
    {
        if (String.IsNullOrEmpty(_hostsFile)) throw new InvalidOperationException("Cannot refresh is no filename was provided");

        LoadHostsFile(_hostsFile);
    }

    /// <summary>
    /// Disposes this hosts file.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Private Methods
    private void LoadHostsFile(string fileName)
    {
        if (!File.Exists(fileName)) throw new ArgumentException($"cannot find hosts file at {fileName}", nameof(fileName));

        _hostsFileLastUpdated = File.GetLastWriteTime(fileName);

        using Stream stream = File.OpenRead(fileName);
        ReadHosts(stream);
    }

    private void ReadHosts(Stream hosts)
    {
        Entries = new List<HostEntry>();
        using StreamReader reader = new(hosts);
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine()?.Trim();

            if (line == null) break;

            Regex blankRegex = new(@"^$");
            Match match = blankRegex.Match(line);

            if (match.Success)
            {
                Entries.Add(new HostEntry());
                continue;
            }

            Regex commentEntryCommentMatch = new(@"^[#]{1}?([^\s]*)\s?([^#]+)#{1}?(.*)$");
            match = commentEntryCommentMatch.Match(line);
            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    Entries.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Value.Trim(), Comment = match.Groups[3].Value.Trim(), IsCommented = true });
                }
                else
                {
                    Entries.Add(new HostEntry(line[1..]));
                }
                continue;
            }

            Regex commentEntryMatch = new(@"^[#]{1}?([^\s]*)\s?([^#]+)$");
            match = commentEntryMatch.Match(line);
            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    Entries.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Value.Trim(), IsCommented = true });
                }
                else
                {
                    Entries.Add(new HostEntry(line[1..]));
                }
                continue;
            }

            Regex commentMatch = new(@"^[#]{1}?(.*)$");
            match = commentMatch.Match(line);

            if (match.Success)
            {
                Entries.Add(new HostEntry(match.Groups[1].Value));
                continue;
            }

            Regex entryCommentMatch = new(@"^?([^\s|#]+)\s?([^#]+)[#]{1}?(#*.*)$");
            match = entryCommentMatch.Match(line);

            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    Entries.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Value.Trim(), Comment = match.Groups[3].Value.Trim() });
                }
                else
                {
                    throw new FormatException(String.Format("Unexpected entry in hosts file: {0}", line));
                }
                continue;
            }

            Regex entryMatch = new(@"^?([^\s|#]+)\s?([^#]+)$");
            match = entryMatch.Match(line);

            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    Entries.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Value.Trim() });
                }
                else
                {
                    throw new FormatException(String.Format("Unexpected entry in hosts file: {0}", line));
                }
                continue;
            }

            throw new FormatException(String.Format("Unexpected entry in hosts file: {0}", line));
        }
    }

    private string GetRaw()
    {
        StringBuilder raw = new();

        foreach (HostEntry entry in Entries)
        {
            raw.AppendLine(entry.ToString());
        }

        return raw.ToString();
    }

    private void PollTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        DateTime lastWriteTime = File.GetLastWriteTime(_hostsFile!);
        if (lastWriteTime > _hostsFileLastUpdated)
        {
            _hostsFileLastUpdated = lastWriteTime;
            Refresh();
            OnHostsFileChanged();
        }
    }

    private void OnHostsFileChanged()
    {
        HostsFileChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _pollTimer?.Dispose();
            }
            _disposed = true;
        }
    }
    #endregion
}
