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

### Modules

Register modules in your ASP.NET Core application:

```csharp
using Asm.AspNetCore.Api.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterModules(() => 
{
    new MyModule(),
});
```

Map module endpoints:

```csharp
var app = builder.Build();

app.MapModuleEndpoints();
```

### RouteHandlerBuilderExtensions

Add validation to route handlers:

```csharp
using Asm.AspNetCore.Builder;
using FluentValidation;
using Microsoft.AspNetCore.Builder;

public class MyValidator : AbstractValidator<MyModel>
{
    public MyValidator() { RuleFor(x => x.Name).NotEmpty(); } 
}

var builder = WebApplication.CreateBuilder(args); var app = builder.Build();

app.MapPost("/endpoint", (MyModel model) => { /* ... */ }) .WithValidation<MyValidator>();

app.Run();
```

### ProblemDetails Factory

Use the custom `ProblemDetailsFactory` to standardize error responses:

```csharp
using Microsoft.AspNetCore.Mvc; using Asm.AspNetCore.Api;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddProblemDetailsFactory();

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

### Authentication

Configure JWT Bearer authentication in your ASP.NET Core application:

```csharp
using Asm.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-authority.com";
        options.Audience = "your-audience";
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

### Authorisation

Add authorization policies to your application:

```csharp
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
        
    options.AddPolicy("RequireScope", policy =>
        policy.RequireClaim("scope", "api.read"));
});

var app = builder.Build();

app.UseAuthorization();

// Use on endpoints
app.MapGet("/admin", () => "Admin content")
    .RequireAuthorization("AdminOnly");

app.Run();
```


## Dependencies

The `Asm.AspNetCore` library depends on the following packages:

- `Azure.Monitor.OpenTelemetry.AspNetCore`
- `Azure.Monitor.OpenTelemetry.Exporter`
- `FluentValidation`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.Azure.WebJobs.Extensions`
- `OpenTelemetry.Exporter.Console`
- `OpenTelemetry.Exporter.OpenTelemetryProtocol`
- `OpenTelemetry.Extensions.Hosting`

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
