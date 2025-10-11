# Asm.Logging

The `Asm.Logging` library provides extensions and utilities for logging in .NET applications using Microsoft.Extensions.Logging. It includes bootstrap logger functionality and Seq integration for structured logging.

## Features

- **Bootstrap Logger Factory**: Create simple loggers before the full dependency injection container is configured
- **Seq Integration**: Built-in support for Seq structured logging
- **Console and Debug Logging**: Standard console and debug output providers
- **Flexible Configuration**: Configure logging with environment-based settings
- **Microsoft.Extensions.Logging**: Based on the standard .NET logging abstractions

## Installation

To install the `Asm.Logging` library, use the .NET CLI:

`dotnet add package Asm.Logging`

Or via the NuGet Package Manager:

`Install-Package Asm.Logging`

## Usage

### Creating a Bootstrap Logger

Use the bootstrap logger factory to create simple loggers during application startup:

```csharp
using Asm.Logging;
using Microsoft.Extensions.Logging;

// Create a bootstrap logger before DI is configured
using var loggerFactory = BootstrapLoggerFactory.Create("MyApp");
var logger = loggerFactory.CreateLogger("Startup");

logger.LogInformation("Application starting...");

try
{
    // Application initialization code
    var builder = WebApplication.CreateBuilder(args);
    // ...
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application failed to start");
    throw;
}
```

### Bootstrap Logger with Environment Detection

The bootstrap logger automatically detects the environment:

```csharp
using Asm.Logging;

// Automatically detects DOTNET_ENVIRONMENT or ASPNETCORE_ENVIRONMENT
var loggerFactory = BootstrapLoggerFactory.Create("MyApp");

// In Development:
// - Includes debug logging
// - Lower minimum log level
// - Console output

// In Production:
// - Information level and above
// - Optimized for performance
```

### Integrating with Seq

Configure Seq logging using environment variables:

```bash
# Set environment variables
export Seq:Host=http://localhost:5341
export Seq:APIKey=your-api-key-here
```

The bootstrap logger will automatically connect to Seq if configured:

```csharp
using Asm.Logging;

// Automatically connects to Seq if Seq:Host is set
var loggerFactory = BootstrapLoggerFactory.Create("MyApp");
var logger = loggerFactory.CreateLogger("Startup");

// Logs are sent to Seq with structured data
logger.LogInformation("User {UserId} logged in from {IPAddress}", userId, ipAddress);
```

### Structured Logging with Scopes

Add contextual information using log scopes:

```csharp
using Asm.Logging;
using Microsoft.Extensions.Logging;

var loggerFactory = BootstrapLoggerFactory.Create("MyApp");
var logger = loggerFactory.CreateLogger("Orders");

using (logger.BeginScope(new Dictionary<string, object>
{
    ["OrderId"] = orderId,
    ["CustomerId"] = customerId
}))
{
    logger.LogInformation("Processing order");
    // All logs within this scope include OrderId and CustomerId
}
```

### Custom Log Levels

Configure minimum log levels:

```csharp
using Asm.Logging;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
    
    // Add Seq if configured
    var seqHost = Environment.GetEnvironmentVariable("Seq:Host");
    if (!string.IsNullOrEmpty(seqHost))
    {
        builder.AddSeq(seqHost, Environment.GetEnvironmentVariable("Seq:APIKey"));
    }
});
```

### Using with ASP.NET Core

Integrate with ASP.NET Core's logging infrastructure:

```csharp
using Asm.Logging;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Clear default providers and add custom configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add Seq
var seqHost = builder.Configuration["Seq:Host"];
if (!string.IsNullOrEmpty(seqHost))
{
    builder.Logging.AddSeq(seqHost, builder.Configuration["Seq:APIKey"]);
}

var app = builder.Build();
app.Run();
```

### Environment-Specific Configuration

Configure logging based on the environment:

```csharp
using Asm.Logging;

var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    
    if (env == "Development")
    {
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddDebug();
    }
    else
    {
        builder.SetMinimumLevel(LogLevel.Information);
    }
    
    // Add Seq in all environments if configured
    var seqHost = Environment.GetEnvironmentVariable("Seq:Host");
    if (!string.IsNullOrEmpty(seqHost))
    {
        builder.AddSeq(seqHost, Environment.GetEnvironmentVariable("Seq:APIKey"));
    }
});
```

## Configuration

### Environment Variables

- `DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT`: The application environment (Development, Production, etc.)
- `Seq:Host`: The Seq server URL (e.g., `http://localhost:5341`)
- `Seq:APIKey`: Optional API key for Seq authentication

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "Seq": {
    "Host": "http://localhost:5341",
    "APIKey": "your-api-key"
  }
}
```

## Best Practices

1. **Use Structured Logging**: Use message templates with parameters instead of string interpolation
2. **Add Context**: Use log scopes to add contextual information
3. **Configure Seq**: Set up Seq for centralized log aggregation and analysis
4. **Environment-Specific Levels**: Use different log levels for different environments
5. **Dispose Loggers**: Properly dispose of logger factories when done

## Dependencies

The `Asm.Logging` library depends on the following packages:

- `Microsoft.Extensions.Logging`
- `Microsoft.Extensions.Logging.Configuration`
- `Microsoft.Extensions.Logging.Console`
- `Microsoft.Extensions.Logging.Debug`
- `Seq.Extensions.Logging`

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.