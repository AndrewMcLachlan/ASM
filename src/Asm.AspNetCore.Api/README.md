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

### Standard Open API Configuration

Add Swagger with a set of simple, but opinionated, default:

```csharp
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddStandardSwaggerGen("My API");
```

### Metadata Endpoint

Add a metadata endpoint to your API:

```csharp
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapMeta();
```    

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
