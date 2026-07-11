# Asm.Win32

The Asm.Win32 project provides wrappers for Win32 API functions, making it easier to interact with Windows-specific functionality from .NET applications.

## Features

- **Win32 API Wrappers**: Wrappers for various Win32 API functions to simplify their usage in .NET applications.
- **Hosts File Management**: Read, write, and monitor the Windows hosts file.

## Installation

To install the Asm.Win32 library, you can use the .NET CLI:

`dotnet add package Asm.Win32`

Or via the NuGet Package Manager:

`Install-Package Asm.Win32`

## Usage

### Hosts File Management

The `Hosts` class provides a convenient way to manage the Windows hosts file:

```csharp
using Asm.Win32;
using System.Net;

// Access the current Windows hosts file
var hosts = Hosts.Current;

// Read all entries
foreach (var entry in hosts.Entries)
{
    if (entry.Address != null)
    {
        Console.WriteLine($"{entry.Address} {entry.Alias} # {entry.Comment}");
    }
}

// Add a new entry (Entries is a read-only snapshot; mutate via the methods below)
hosts.AddEntry(new HostEntry
{
    Address = IPAddress.Parse("192.168.1.100"),
    Alias = "myserver.local",
    Comment = "Development server"
});

// Write changes back to the hosts file
hosts.WriteHostsToFile();
```

### Reading Hosts from a Custom File

```csharp
using Asm.Win32;

// Load hosts from a custom file
var hosts = new Hosts(@"C:\custom\hosts");

// Access entries
var raw = hosts.Raw;
Console.WriteLine(raw);
```

### Reading Hosts from a Stream

```csharp
using Asm.Win32;

using (var stream = File.OpenRead("hosts.txt"))
{
    var hosts = new Hosts(stream);
    
    foreach (var entry in hosts.Entries)
    {
        // Process entries
    }
}
```

### Monitoring Hosts File Changes

The `Hosts.Current` instance automatically monitors the Windows hosts file for changes:

```csharp
using Asm.Win32;

var hosts = Hosts.Current;

hosts.HostsFileChanged += (sender, e) =>
{
    Console.WriteLine("Hosts file has been modified!");
    hosts.Refresh(); // Reload from disk
};

// The file is polled every 10 seconds for changes
```

### Working with Host Entries

```csharp
using Asm.Win32;
using System.Net;

// Create different types of entries
var hosts = new Hosts(@"C:\temp\hosts");

// Regular entry
var entry1 = new HostEntry
{
    Address = IPAddress.Parse("127.0.0.1"),
    Alias = "localhost"
};

// Entry with comment
var entry2 = new HostEntry
{
    Address = IPAddress.Parse("192.168.1.1"),
    Alias = "router.local",
    Comment = "Home router"
};

// Commented out entry (disabled)
var entry3 = new HostEntry
{
    Address = IPAddress.Parse("10.0.0.5"),
    Alias = "oldserver.local",
    IsCommented = true
};

// Comment line only
var commentEntry = new HostEntry("This is a section header");

// Blank line
var blankEntry = new HostEntry();

hosts.AddEntry(entry1);
hosts.AddEntry(entry2);
hosts.AddEntry(entry3);
hosts.AddEntry(commentEntry);
hosts.AddEntry(blankEntry);

// Other mutators: InsertEntry(index, entry), UpdateEntry(index, entry),
// RemoveEntry(entry), RemoveEntryAt(index), ClearEntries()

// Save to file
hosts.WriteHostsToFile(@"C:\temp\hosts");
```

### Refreshing from Disk

```csharp
using Asm.Win32;

var hosts = Hosts.Current;

// Make external changes to hosts file...

// Reload from disk (discards any unsaved changes)
hosts.Refresh();
```

### Writing to a Stream

```csharp
using Asm.Win32;

var hosts = Hosts.Current;

using (var stream = File.Create("hosts-backup.txt"))
{
    hosts.WriteHosts(stream);
}
```

## Important Notes

- **Administrator Privileges**: Writing to the Windows hosts file requires administrator privileges
- **File Location**: The Windows hosts file is typically located at `%SystemRoot%\system32\drivers\etc\hosts`
- **Automatic Monitoring**: `Hosts.Current` automatically monitors the file for external changes every 10 seconds
- **Windows only**: `Hosts` is annotated `[SupportedOSPlatform("windows")]`; calling it from non-Windows-scoped code raises analyzer warning CA1416
- **Read-only entries**: `Entries` is an `IReadOnlyList<HostEntry>` snapshot — mutate with `AddEntry`/`InsertEntry`/`UpdateEntry`/`RemoveEntry`/`RemoveEntryAt`/`ClearEntries`
- **Thread Safety**: reads of `Entries` and the background `Refresh` are safe to run concurrently; each `Entries` access returns an independent snapshot
- **Malformed input**: a line that is not blank, not a comment, and not a valid IP entry throws `FormatException` naming the offending line and line number

## HostEntry Properties

- **Address**: The IP address (`IPAddress`)
- **Alias**: The hostname or domain name (`string`)
- **Comment**: Optional comment text (`string`)
- **IsCommented**: Whether the entry is commented out (disabled) (`bool`)

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
