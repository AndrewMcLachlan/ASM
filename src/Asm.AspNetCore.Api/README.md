# Asm.AspNetCore.Api

The `Asm.AspNetCore.Api` project provides foundational components and utilities for building ASP.NET Core APIs. It includes features such as custom middleware, API-specific extensions, and utilities to streamline API development.

## Features

- **Custom Middleware**: Middleware for handling common API concerns such as error handling and logging.
- **ProblemDetails Factory**: Custom implementation of `ProblemDetailsFactory` for consistent error responses.
- **Validation Utilities**: Tools for integrating FluentValidation and other validation frameworks.
- **API Extensions**: Helper methods and extensions for configuring and enhancing ASP.NET Core APIs.

## Installation

To install the `Asm.AspNetCore.Api` library, you can use the .NET CLI:

`dotnet add package Asm.AspNetCore.Api`

Or via the NuGet Package Manager:

`Install-Package Asm.AspNetCore.Api`

## Usage

### Custom Middleware

Add custom middleware to handle exceptions and provide consistent error responses:

```csharp
using Asm.AspNetCore.Api.Middleware;
var builder = WebApplication.CreateBuilder(args); var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();
```

### ProblemDetails Factory

Use the custom `ProblemDetailsFactory` to standardize error responses:

```csharp
using Microsoft.AspNetCore.Mvc; using Asm.AspNetCore.Api;
var builder = WebApplication.CreateBuilder(args); builder.Services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
var app = builder.Build(); app.Run();
```

### Validation Utilities

Integrate FluentValidation for model validation:

```csharp
using FluentValidation; using Asm.AspNetCore.Api.Validation;
public class MyModelValidator : AbstractValidator<MyModel> { public MyModelValidator() { RuleFor(x => x.Name).NotEmpty(); } }
var builder = WebApplication.CreateBuilder(args); builder.Services.AddValidatorsFromAssemblyContaining<MyModelValidator>();
var app = builder.Build(); app.Run();
```

### API Extensions

Simplify API configuration with helper methods:

```csharp
using Asm.AspNetCore.Api.Extensions;
var builder = WebApplication.CreateBuilder(args); builder.Services.AddApiServices();
var app = builder.Build(); app.UseApiDefaults();
app.Run();
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
