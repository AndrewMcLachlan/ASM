# Asm.Cqrs

The `Asm.Cqrs` project provides a lightweight framework for implementing the Command Query Responsibility Segregation (CQRS) pattern in .NET applications. It includes abstractions and utilities for handling commands, queries, and events in a clean and maintainable way.

## Features

- **Command Handling**: Define and process commands with dedicated handlers.
- **Query Handling**: Simplify query execution with query handlers.
- **Event Dispatching**: Publish and handle domain events.
- **Pipeline Behaviors**: Add cross-cutting concerns (e.g., logging, validation) to command and query pipelines.
- **Dependency Injection Integration**: Seamless integration with ASP.NET Core's DI container.

## Installation

To install the `Asm.Cqrs` library, use the .NET CLI:

`dotnet add package Asm.Cqrs`

Or via the NuGet Package Manager:

`Install-Package Asm.Cqrs`

## Usage

### Defining a Command

Create a command by implementing the `ICommand` interface:

```csharp
using Asm.Cqrs;
public class CreateOrderCommand : ICommand { public string OrderId { get; set; } public string CustomerName { get; set; } }
```

### Handling a Command

Create a command handler by implementing the `ICommandHandler<TCommand>` interface:

```csharp
using Asm.Cqrs;
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand> { public Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken) { // Handle the command logic here Console.WriteLine($"Order created for {command.CustomerName}"); return Task.CompletedTask; } }
```

### Defining a Query

Create a query by implementing the `IQuery<TResult>` interface:

```csharp
using Asm.Cqrs;
public class GetOrderQuery : IQuery<Order> { public string OrderId { get; set; } }
```

### Handling a Query

Create a query handler by implementing the `IQueryHandler<TQuery, TResult>` interface:

```csharp
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, Order> { public Task<Order> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken) { // Handle the query logic here return Task.FromResult(new Order { OrderId = query.OrderId, CustomerName = "Joe Bloggs" }); } }
```

### Dispatching Commands and Queries

Use the `ICommandDispatcher` and `IQueryDispatcher` to dispatch commands and queries:

```csharp
using Asm.Cqrs;
public class OrderService
{ 
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public OrderService(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    public async Task CreateOrderAsync()
    {
        var command = new CreateOrderCommand { OrderId = "123", CustomerName = "Jane Doe" };
        await _commandDispatcher.DispatchAsync(command);
    }

    public async Task<Order> GetOrderAsync(string orderId)
    {
        var query = new GetOrderQuery { OrderId = orderId };
        return await _queryDispatcher.DispatchAsync(query);
    }
}
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
