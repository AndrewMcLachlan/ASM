# Asm.Cqrs.AspNetCore

The `Asm.Cqrs.AspNetCore` project provides extensions and utilities to integrate the Command Query Responsibility Segregation (CQRS) pattern into ASP.NET Core applications. It simplifies the setup and usage of CQRS in web APIs by providing middleware, model binding, and dependency injection support.

## Features

- **Command and Query Dispatching**: Seamless integration of `ICommandDispatcher` and `IQueryDispatcher` into ASP.NET Core.
- **Model Binding for Commands and Queries**: Automatically bind HTTP request data to commands and queries.
- **Middleware Support**: Add cross-cutting concerns like validation, logging, and exception handling to the CQRS pipeline.
- **Dependency Injection Integration**: Easily register CQRS components in the ASP.NET Core DI container.

## Installation

To install the `Asm.Cqrs.AspNetCore` library, use the .NET CLI:

Or via the NuGet Package Manager:


## Usage

### Registering CQRS Services

In your `Program.cs` or `Startup.cs`, register the CQRS services:

```csharp
using Asm.Cqrs.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
// Register CQRS services builder.Services.AddCqrs();
var app = builder.Build(); app.Run();
```

### Defining a Command and Handler

Define a command and its handler:

```csharp
using Asm.Cqrs;
public class CreateOrderCommand : ICommand { public string OrderId { get; set; } public string CustomerName { get; set; } }
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand> { public Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken) { // Handle the command logic here Console.WriteLine($"Order created for {command.CustomerName}"); return Task.CompletedTask; } }
```


### Defining a Query and Handler

Define a query and its handler:

```csharp
using Asm.Cqrs;
public class GetOrderQuery : IQuery<Order> { public string OrderId { get; set; } }
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, Order> { public Task<Order> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken) { // Handle the query logic here return Task.FromResult(new Order { OrderId = query.OrderId, CustomerName = "John Doe" }); } }
```

### Using Commands and Queries in Controllers

Use commands and queries in your ASP.NET Core controllers:

```csharp
using Asm.Cqrs; using Microsoft.AspNetCore.Mvc;
[ApiController] [Route("api/orders")] public class OrdersController : ControllerBase { private readonly ICommandDispatcher _commandDispatcher; private readonly IQueryDispatcher _queryDispatcher;
public OrdersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
{
    _commandDispatcher = commandDispatcher;
    _queryDispatcher = queryDispatcher;
}

[HttpPost]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
{
    await _commandDispatcher.DispatchAsync(command);
    return CreatedAtAction(nameof(GetOrder), new { orderId = command.OrderId }, command);
}

[HttpGet("{orderId}")]
public async Task<IActionResult> GetOrder(string orderId)
{
    var query = new GetOrderQuery { OrderId = orderId };
    var order = await _queryDispatcher.DispatchAsync(query);
    return Ok(order);
}
}
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
