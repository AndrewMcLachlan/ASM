# Asm.Net

The `Asm.Net` provides helpers for networking.

## Features

**IP Address Extensions**: Helpers for working with IP addresses.


## Installation

To install the `Asm.Net` library, use the .NET CLI:

`dotnet add package Asm.Net`

Or via the NuGet Package Manager:

`Install-Package Asm.Net`

## Usage

### IP Address Extensions

The `Asm.Net` library provides extension methods for `IPAddress` to simplify common tasks:

```csharp
using System.Net;

var ipAddress = new IPAddress(new byte[] { 192, 168, 1, 1 });
var mask = new IPAddress(new byte[] { 255, 255, 255, 0 });

// Convert to CIDR notation
string cidr = ipAddress.ToCidr(mask);

// Convert an IP address to an unsigned integer
uint ipAsUInt = ipAddress.ToUInt();

// Create an IP address from an unsigned integer
IPAddress ipFromUInt = IPAddress.FromUInt(ipAsUInt);

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.