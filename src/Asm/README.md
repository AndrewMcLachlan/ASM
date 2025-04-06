# Asm

The Asm project provides core utilities and extensions for .NET applications. It includes various helper methods and extensions to enhance the functionality of .NET collections and other core features.

## Features

- **IEnumerable Extensions**: Useful extension methods for `IEnumerable<T>`, such as paging, shuffling, and checking for null or empty collections.

## Installation

To install the Asm library, you can use the .NET CLI:

`dotnet add package Asm`

Or via the NuGet Package Manager:

`Install-Package Asm`

## Usage

### IEnumerable Extensions

```csharp
using System.Collections.Generic;
var items = Enumerable.Range(1, 100).ToList();
// Get the second page with 10 items per page var page = items.Page(10, 2);
// Shuffle the items var shuffled = items.Shuffle();
```

#### Paging and Shuffling

Use the `Page` and `Shuffle` extension methods for `IEnumerable<T>`:

```csharp
using System.Collections.Generic;
var items = Enumerable.Range(1, 100).ToList();
// Get the second page with 10 items per page var page = items.Page(10, 2);
// Shuffle the items var shuffled = items.Shuffle();
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
