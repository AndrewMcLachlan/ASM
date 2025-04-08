# Asm.Domain

The `Asm.Domain` project provides a foundation for implementing domain-driven design (DDD) principles in .NET applications. It includes abstractions and base classes for defining entities, value objects, domain events, and aggregates, enabling a clean and maintainable domain layer.

## Features

- **Entities**: Base classes and utilities for defining domain entities with unique identifiers.
- **Domain Events**: Abstractions for publishing and handling domain events.
- **Aggregates**: Base classes for defining aggregate roots and enforcing aggregate boundaries.
- **Specifications**: Support for the specification pattern to encapsulate business rules.

## Installation

To install the `Asm.Domain` library, use the .NET CLI:

`dotnet add package Asm.Domain`

Or via the NuGet Package Manager:

`Install-Package Asm.Domain`

## Usage

### Defining an Entity

Create an entity by inheriting from the `Entity` base class:

```csharp
using Asm.Domain;

[AggregateRoot]
public class Order : Entity
{
    public string CustomerName { get; private set; }

    public Order(Guid id, string customerName) : base(id)
    {
        CustomerName = customerName;
    }
}
```

Entities can be either aggregate roots or regular entities. Use the `[AggregateRoot]` attribute to mark an entity as an aggregate root.

#### Entity Types

- **Entity**: An entity that can raise domain events
- **KeyedEntity**: An entity with a unique identifier
- **NamedEntity**: An entity with a name

### Raising Domain Events

Entities can raise domain events to notify other parts of the system about changes:

```csharp
using Asm.Domain;
public class Order : Entity<Guid>
{ 
    public string CustomerName { get; private set; }
    public Order(Guid id, string customerName) : base(id)
    {
        CustomerName = customerName;
        AddDomainEvent(new OrderCreatedEvent(id));
    }
}
```

### Handling Domain Events

Use `MediatR` to handle domain events:

```csharp
using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class OrderCreatedEvent : INotification
{ 
    public Guid OrderId { get; }
    
    public OrderCreatedEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent> 
{ 
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    { 
        // Handle the event (e.g., send an email, log the event, etc.) 
        return Task.CompletedTask; 
    } 
}
```

### Specifications

Use specifications to apply common patters to queries.

```csharp
using Asm.Domain;

public class RecentOrders : ISpecification<Order>
{
    public IQueryable<Order> Apply(IQueryable<Order> query) =>
        query.Where(o => o.OrderDate > DateTime.UtcNow.AddDays(-30));
}
```

## Contributing

Contributions are welcome! If you have ideas for improvements or find any issues, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
