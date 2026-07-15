# Asm.Cqrs

A lightweight implementation of the Command Query Responsibility Segregation (CQRS) pattern for .NET.
Commands and queries are first-class, separate concepts with their own handler and dispatcher interfaces,
dispatched via cached compiled delegates (no per-call reflection).

## Features

- **Separate command and query abstractions**: `ICommand`, `ICommand<TResponse>` and `IQuery<TResponse>` with matching handler interfaces.
- **Separate dispatchers**: `ICommandDispatcher` and `IQueryDispatcher`, so the write and read sides of an application can be segregated at the injection point.
- **`ValueTask`-based** handler and dispatcher signatures.
- **Dependency injection integration**: register handlers by assembly scan or individually.

## Installation

```
dotnet add package Asm.Cqrs
```

## Usage

### Defining a query

Implement `IQuery<TResponse>`:

```csharp
using Asm.Cqrs.Queries;

public record GetOrder(int Id) : IQuery<Order>;
```

### Handling a query

Implement `IQueryHandler<TQuery, TResponse>`:

```csharp
using Asm.Cqrs.Queries;

public class GetOrderHandler : IQueryHandler<GetOrder, Order>
{
    public ValueTask<Order> Handle(GetOrder query, CancellationToken cancellationToken) =>
        ValueTask.FromResult(new Order { Id = query.Id, CustomerName = "Joe Bloggs" });
}
```

### Defining a command

Implement `ICommand<TResponse>` for a command that returns a response, or `ICommand` for one that does not:

```csharp
using Asm.Cqrs.Commands;

public record CreateOrder(string CustomerName) : ICommand<Order>;

public record DeleteOrder(int Id) : ICommand;
```

### Handling a command

Implement `ICommandHandler<TCommand, TResponse>` or `ICommandHandler<TCommand>`:

```csharp
using Asm.Cqrs.Commands;

public class CreateOrderHandler : ICommandHandler<CreateOrder, Order>
{
    public ValueTask<Order> Handle(CreateOrder command, CancellationToken cancellationToken)
    {
        // Create the order...
    }
}

public class DeleteOrderHandler : ICommandHandler<DeleteOrder>
{
    public ValueTask Handle(DeleteOrder command, CancellationToken cancellationToken)
    {
        // Delete the order...
    }
}
```

### Registering handlers

Register all handlers in an assembly:

```csharp
builder.Services.AddCommandHandlers(typeof(CreateOrder).Assembly);
builder.Services.AddQueryHandlers(typeof(GetOrder).Assembly);
```

Or register handlers individually:

```csharp
builder.Services.AddCommandHandler<CreateOrderHandler, CreateOrder, Order>();
builder.Services.AddCommandHandler<DeleteOrderHandler, DeleteOrder>();
builder.Services.AddQueryHandler<GetOrderHandler, GetOrder, Order>();
```

### Dispatching

Inject `IQueryDispatcher` and `ICommandDispatcher`:

```csharp
public class OrderService(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
{
    public async Task<Order> CreateOrderAsync(string customerName, CancellationToken cancellationToken) =>
        await commandDispatcher.Dispatch(new CreateOrder(customerName), cancellationToken);

    public async Task DeleteOrderAsync(int id, CancellationToken cancellationToken) =>
        await commandDispatcher.Execute(new DeleteOrder(id), cancellationToken);

    public async Task<Order> GetOrderAsync(int id, CancellationToken cancellationToken) =>
        await queryDispatcher.Dispatch(new GetOrder(id), cancellationToken);
}
```

Commands that return a response are dispatched with `Dispatch`; commands that do not are executed with `Execute`.

## ASP.NET Core

See [Asm.Cqrs.AspNetCore](https://github.com/AndrewMcLachlan/ASM/tree/main/src/Asm.Cqrs.AspNetCore) to map
commands and queries directly to minimal API endpoints.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
