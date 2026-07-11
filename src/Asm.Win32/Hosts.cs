using System.Net;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Asm.Win32;

/// <summary>
/// Class for controlling the Windows Hosts file.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed partial class Hosts : IDisposable
{
    #region Constants
    private static readonly string DefaultHostsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
    #endregion

    #region Events
    /// <summary>
    /// Fired when the Windows hosts file has been changed.
    /// </summary>
    public event EventHandler? HostsFileChanged;
    #endregion

    #region Fields
    private static Hosts? _instance;
    private static readonly Lock _lock = new();
    private static string _systemHostsFile = DefaultHostsFile;
    private readonly Lock _entriesLock = new();
    private readonly List<HostEntry> _entries = [];
    private readonly string? _hostsFile;
    private readonly Timer? _pollTimer;
    private DateTime _hostsFileLastUpdated;
    private bool _disposed;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the path to the system hosts file.
    /// </summary>
    /// <remarks>
    /// This property can be set to a custom path for testing purposes.
    /// Setting this property resets the <see cref="Current"/> singleton instance.
    /// </remarks>
    public static string SystemHostsFile
    {
        get => _systemHostsFile;
        set
        {
            _systemHostsFile = value;
            ResetInstance();
        }
    }

    /// <summary>
    /// The current Windows hosts file.
    /// </summary>
    public static Hosts Current
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new Hosts();
                }
            }
            return _instance;
        }
    }

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
    /// <remarks>
    /// This is a read-only snapshot taken at the point of access. Use
    /// <see cref="AddEntry(HostEntry)"/>, <see cref="InsertEntry(int, HostEntry)"/>,
    /// <see cref="UpdateEntry(int, HostEntry)"/>, <see cref="RemoveEntry(HostEntry)"/>,
    /// <see cref="RemoveEntryAt(int)"/> and <see cref="ClearEntries"/> to mutate the
    /// entry collection.
    /// </remarks>
    public IReadOnlyList<HostEntry> Entries
    {
        get
        {
            lock (_entriesLock)
            {
                return _entries.ToArray();
            }
        }
    }
    #endregion

    #region Constructors
    private Hosts() : this(SystemHostsFile)
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

    #endregion

    #region Public Methods
    /// <summary>
    /// Resets the singleton instance, allowing it to be recreated with a new hosts file path.
    /// </summary>
    /// <remarks>
    /// This method is primarily intended for testing purposes.
    /// </remarks>
    public static void ResetInstance()
    {
        Hosts? instance;
        lock (_lock)
        {
            instance = _instance;
            _instance = null;
        }
        instance?.Dispose();
    }

    /// <summary>
    /// Adds a host entry to the end of the collection.
    /// </summary>
    /// <param name="entry">The entry to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <see langword="null"/>.</exception>
    public void AddEntry(HostEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_entriesLock)
        {
            _entries.Add(entry);
        }
    }

    /// <summary>
    /// Inserts a host entry at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which the entry should be inserted.</param>
    /// <param name="entry">The entry to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the bounds of the collection.</exception>
    public void InsertEntry(int index, HostEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_entriesLock)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)_entries.Count, nameof(index));
            _entries.Insert(index, entry);
        }
    }

    /// <summary>
    /// Replaces the host entry at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the entry to replace.</param>
    /// <param name="entry">The replacement entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the bounds of the collection.</exception>
    public void UpdateEntry(int index, HostEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_entriesLock)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)_entries.Count, nameof(index));
            _entries[index] = entry;
        }
    }

    /// <summary>
    /// Removes the first occurrence of the specified host entry.
    /// </summary>
    /// <param name="entry">The entry to remove.</param>
    /// <returns><see langword="true"/> if the entry was found and removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <see langword="null"/>.</exception>
    public bool RemoveEntry(HostEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_entriesLock)
        {
            return _entries.Remove(entry);
        }
    }

    /// <summary>
    /// Removes the host entry at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the entry to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the bounds of the collection.</exception>
    public void RemoveEntryAt(int index)
    {
        lock (_entriesLock)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)_entries.Count, nameof(index));
            _entries.RemoveAt(index);
        }
    }

    /// <summary>
    /// Removes all host entries.
    /// </summary>
    public void ClearEntries()
    {
        lock (_entriesLock)
        {
            _entries.Clear();
        }
    }

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
    /// Writes the host entries back to the file this instance was loaded from,
    /// or to the system hosts file if this instance was not loaded from a file.
    /// </summary>
    public void WriteHostsToFile()
    {
        WriteHostsToFile(_hostsFile ?? SystemHostsFile);
        OnHostsFileChanged();
    }

    /// <summary>
    /// Writes the hosts entries to given file.
    /// </summary>
    /// <param name="fileName">The name of the file to write to.</param>
    public void WriteHostsToFile(string fileName)
    {
        using (FileStream stream = File.Create(fileName))
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
        if (String.IsNullOrEmpty(_hostsFile)) throw new InvalidOperationException("Cannot refresh if no filename was provided");

        LoadHostsFile(_hostsFile);
    }

    /// <summary>
    /// Disposes this hosts file.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _pollTimer?.Dispose();
        lock (_lock)
        {
            if (ReferenceEquals(_instance, this))
            {
                _instance = null;
            }
        }
        _disposed = true;
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
        // Parse into a local list first, then swap it in atomically under the lock so
        // concurrent readers (and the poll timer's Refresh) never observe a partially
        // built collection.
        List<HostEntry> parsed = [];
        using StreamReader reader = new(hosts);
        int lineNumber = 0;
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine()?.Trim();

            if (line == null) break;

            lineNumber++;

            Match match = BlankRegex().Match(line);

            if (match.Success)
            {
                parsed.Add(new HostEntry());
                continue;
            }

            match = CommentedEntryWithCommentRegex().Match(line);
            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    parsed.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Value.Trim(), Comment = match.Groups[3].Value.Trim(), IsCommented = true });
                }
                else
                {
                    parsed.Add(new HostEntry(line[1..]));
                }
                continue;
            }

            match = CommentedEntryRegex().Match(line);
            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    parsed.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Value.Trim(), IsCommented = true });
                }
                else
                {
                    parsed.Add(new HostEntry(line[1..]));
                }
                continue;
            }

            match = CommentRegex().Match(line);

            if (match.Success)
            {
                parsed.Add(new HostEntry(match.Groups[1].Value));
                continue;
            }

            match = EntryWithCommentRegex().Match(line);

            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    parsed.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Value.Trim(), Comment = match.Groups[3].Value.Trim() });
                }
                else
                {
                    throw MalformedEntry(lineNumber, line);
                }
                continue;
            }

            match = EntryRegex().Match(line);

            if (match.Success)
            {

                if (IPAddress.TryParse(match.Groups[1].Value.Trim(), out IPAddress? ip))
                {
                    parsed.Add(new HostEntry { Address = ip, Alias = match.Groups[2].Success ? match.Groups[2].Value.Trim() : null });
                }
                else
                {
                    throw MalformedEntry(lineNumber, line);
                }
                continue;
            }

            throw MalformedEntry(lineNumber, line);
        }

        lock (_entriesLock)
        {
            _entries.Clear();
            _entries.AddRange(parsed);
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

    private static FormatException MalformedEntry(int lineNumber, string line) =>
        new($"Malformed host entry at line {lineNumber}: '{line}'. Expected an IP address followed by an optional alias and comment (e.g. '127.0.0.1 localhost #comment').");

    [GeneratedRegex(@"^$")]
    private static partial Regex BlankRegex();

    [GeneratedRegex(@"^[#]{1}?([^\s]*)\s?([^#]+)#{1}?(.*)$")]
    private static partial Regex CommentedEntryWithCommentRegex();

    [GeneratedRegex(@"^[#]{1}?([^\s]*)\s?([^#]+)$")]
    private static partial Regex CommentedEntryRegex();

    [GeneratedRegex(@"^[#]{1}?(.*)$")]
    private static partial Regex CommentRegex();

    [GeneratedRegex(@"^([^\s#]+)\s+([^#]+)#(.*)$")]
    private static partial Regex EntryWithCommentRegex();

    [GeneratedRegex(@"^([^\s#]+)(?:\s+([^#]+))?$")]
    private static partial Regex EntryRegex();
    #endregion
}
