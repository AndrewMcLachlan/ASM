# Asm.Hosting

The `Asm.Hosting` is an opinionated bootstrapping library for .NET hosted applications, designed to simplify the setup and configuration of hosting environments.

## Features

**Boostrap application**: The library provides a simple way to bootstrap your hosted application with minimal configuration.


## Installation

To install the `Asm.Hosting` library, use the .NET CLI:

`dotnet add package Asm.Hosting`

Or via the NuGet Package Manager:

`Install-Package Asm.Hosting`

## Usage

### Running a Hosted Application

```csharp
using Asm.Hosting;

return await AppStart.RunAsync(args, "MyApp", 
    builder =>
    {
        // Configure services
        builder.Services.AddLogging();
        builder.Services.AddHostedService<MyHostedService>();
    },
    app =>
    {
        // Configure the application
        app.Run(async context =>
        {
            await context.Response.WriteAsync("Hello, World!");
        });
    });
```

## Dependencies

The `Asm.Hosting` library depends on the following packages:

- `Microsoft.Extensions.Hosting`
- `Microsoft.Extensions.Configuration.Binder`
- `Serilog`
- `Serilog.Extensions.Hosting`
- `System.Text.Json`

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.