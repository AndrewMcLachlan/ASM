# Asm.AspNetCore.Modules

The `Asm.AspNetCore.Modules` project provides a modular framework for ASP.NET Core applications, enabling developers to organize application features into independent, reusable modules. This approach improves scalability, maintainability, and separation of concerns.

## Features

- **Modular Architecture**: Build applications with a modular design for better organization and flexibility.
- **Dynamic Module Loading**: Load and configure modules dynamically at runtime.
- **Module Lifecycle Management**: Support for initializing, configuring, and disposing of modules.
- **Dependency Injection Integration**: Seamless integration with ASP.NET Core's DI container.
- **Middleware Support**: Modules can define their own middleware pipelines.

## Installation

To install the `Asm.AspNetCore.Modules` library, use the .NET CLI:
Or via the NuGet Package Manager:
## Usage

### Defining a Module

Create a module by implementing the `IModule` interface:

```csharp
using Asm.AspNetCore.Modules;

public class MyModule : IModule
{ 
    public void ConfigureServices(IServiceCollection services) 
    { // Register services for this module 
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure middleware for this module
    }
}
```

### Registering Modules

Register modules in your `Program.cs` or `Startup.cs`:

```csharp
using Asm.AspNetCore.Modules;
var builder = WebApplication.CreateBuilder(args);
// Register modules builder.Services.AddModules();
var app = builder.Build();
// Use modules app.UseModules();
app.Run();
```

### Dynamic Module Loading

Modules can be loaded dynamically from assemblies:

```csharp
builder.Services.AddModulesFromAssembly(typeof(MyModule).Assembly);
```

### Module Lifecycle

Modules can define logic for initialization, configuration, and cleanup:

```csharp
public class MyModule : IModule { public void ConfigureServices(IServiceCollection services) { // Initialization logic }
public void Configure(IApplicationBuilder app)
{
    // Middleware configuration
}

public void Dispose()
{
    // Cleanup logic
}
}
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
