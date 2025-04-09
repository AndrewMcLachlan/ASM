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

`dotnet add package Asm.AspNetCore.Modules`

Or via the NuGet Package Manager:

`Install-Package Asm.AspNetCore.Modules`

## Usage

### Defining a Module

Create a module by implementing the `IModule` interface:

```csharp
public class MyModule : IModule
{ 
    public IServiceCollection AddServices(IServiceCollection services)
    {
        // Initialization logic
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Endpoint configuration
    }
}
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
