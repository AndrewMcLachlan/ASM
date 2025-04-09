# Asm.Serilog

The `Asm.Serilog` library provides pre-configured Serilog integrations and utilities to simplify logging in .NET applications. It is designed to work seamlessly with the Microsoft.Extensions.Hosting framework and other Asm libraries.

## Features

- **Pre-configured Serilog Setup**: Simplifies the configuration of Serilog for .NET applications.
- **Support for Common Sinks**: Easily configure sinks like Console, File, and Seq.
- **Enrichment Utilities**: Add contextual information to logs, such as application name, environment, and more.
- **Integration with Microsoft.Extensions.Hosting**: Streamlined logging setup for hosted applications.

## Installation

To install the `Asm.Serilog` library, use the .NET CLI:

`dotnet add package Asm.Serilog`

Or via the NuGet Package Manager:

`Install-Package Asm.Serilog`

## Usage

### Setting Up Serilog in a Hosted Application

The `Asm.Serilog` library simplifies the setup of Serilog in your application:

```csharp
using Asm.Serilog;
using Microsoft.Extensions.Hosting;
var host = Host.CreateDefaultBuilder(args);
host.UseCustomSerilog();
host.Build().Run();
```

### Seq Integration

To send logs to Seq, configure the `Seq:Host` and `Seq:ApiKey` values.


## Dependencies

The `Asm.Serilog` library depends on the following packages:

- `Serilog`
- `Serilog.Extensions.Hosting`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`
- `Serilog.Sinks.Seq`

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
