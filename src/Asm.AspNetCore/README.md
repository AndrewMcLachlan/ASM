# Asm.AspNetCore

The Asm.AspNetCore project provides utilities and extensions for ASP.NET Core applications. It includes features for route handling, validation, and OpenTelemetry integration.

## Features

- **RouteHandlerBuilderExtensions**: Extension methods for adding validation to route handlers.
- **OpenTelemetry Integration**: Processors and utilities for integrating with OpenTelemetry in ASP.NET Core applications.

## Installation

To install the Asm.AspNetCore library, you can use the .NET CLI:

`dotnet add package Asm.AspNetCore`

Or via the NuGet Package Manager:

`Install-Package Asm.AspNetCore`

## Usage

### RouteHandlerBuilderExtensions

Add validation to route handlers:

```csharp
using Asm.AspNetCore.Builder; using FluentValidation; using Microsoft.AspNetCore.Builder;
public class MyValidator : AbstractValidator<MyModel> { public MyValidator() { RuleFor(x => x.Name).NotEmpty(); } }
var builder = WebApplication.CreateBuilder(args); var app = builder.Build();
app.MapPost("/endpoint", (MyModel model) => { /* ... */ }) .WithValidation<MyValidator>();
app.Run();
```

### OpenTelemetry Integration

#### HttpContextLogProcessor

Add custom attributes to log records based on the current HTTP context:

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
