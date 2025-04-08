# Asm.AspNetCore.Mvc

The `Asm.AspNetCore.Mvc` project provides extensions and utilities to enhance ASP.NET Core MVC applications. It includes features for improving controller functionality, model binding, and response handling.

## Features

- **Controller Extensions**: Simplify common controller tasks and improve maintainability.
- **Model Binding Enhancements**: Custom model binders for advanced scenarios.
- **Action Result Utilities**: Helpers for creating consistent and reusable action results.
- **Validation Support**: Tools for integrating validation frameworks like FluentValidation.

## Installation

To install the `Asm.AspNetCore.Mvc` library, use the .NET CLI:

`dotnet add package Asm.AspNetCore.Mvc`

Or via the NuGet Package Manager:

`Install-Package Asm.AspNetCore.Mvc`

## Usage

### Controller Extensions

Enhance your controllers with additional functionality:

```csharp
using Asm.AspNetCore.Mvc;
public class MyController : ControllerBase { public IActionResult Get() { return this.OkWithMetadata(new { Message = "Success" }); } }
```

### Custom Model Binding

Use custom model binders for advanced binding scenarios:

```csharp
using Asm.AspNetCore.Mvc.ModelBinding;
public class MyModelBinder : IModelBinder { public Task BindModelAsync(ModelBindingContext bindingContext) { // Custom binding logic return Task.CompletedTask; } }
```

### Action Result Utilities

Create consistent action results with helper methods:

```csharp
using Asm.AspNetCore.Mvc;
public class MyController : ControllerBase { public IActionResult Get() { return this.CreatedWithMetadata(new { Id = 1 }, "Resource created successfully"); } }
```

### Validation Integration

Integrate FluentValidation for model validation:

```csharp
using FluentValidation; using Asm.AspNetCore.Mvc.Validation;
public class MyModelValidator : AbstractValidator<MyModel> { public MyModelValidator() { RuleFor(x => x.Name).NotEmpty(); } }
var builder = WebApplication.CreateBuilder(args); builder.Services.AddValidatorsFromAssemblyContaining<MyModelValidator>();
var app = builder.Build(); app.Run();
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
