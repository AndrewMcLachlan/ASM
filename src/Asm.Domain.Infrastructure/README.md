# Asm.Domain.Infrastructure

The `Asm.Domain.Infrastructure` library provides infrastructure components for implementing domain-driven design (DDD) with Entity Framework Core. It bridges the domain layer with the persistence layer, offering repository patterns, domain event handling, and database context utilities.

## Features

- **DomainDbContext**: A specialized `DbContext` that automatically publishes domain events when saving changes
- **Repository Base Classes**: Generic repository implementations for common data access patterns
- **Entity Framework Core Extensions**: Extension methods for working with EF Core `DbSet` and `IQueryable`
- **Domain Event Publisher**: Infrastructure for publishing and handling domain events
- **Specification Pattern Support**: Query extensions for applying specifications
- **OpenTelemetry Integration**: Built-in instrumentation for Entity Framework Core and SQL Client
- **Caching Support**: Integration with LazyCache for optimizing query performance

## Installation

To install the `Asm.Domain.Infrastructure` library, use the .NET CLI:

`dotnet add package Asm.Domain.Infrastructure`

Or via the NuGet Package Manager:

`Install-Package Asm.Domain.Infrastructure`

## Usage

### Setting Up DomainDbContext

Create a custom `DbContext` that inherits from `DomainDbContext`:

```csharp
using Asm.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DomainDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options, IPublisher publisher) 
        : base(options, publisher)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure your entities
    }
}
```

### Registering Infrastructure Services

Register the infrastructure services in your application:

```csharp
using Asm.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add domain infrastructure services
builder.Services.AddDomainInfrastructure<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.Run();
```

### Using Repository Base Classes

Create repositories using the provided base classes:

```csharp
using Asm.Domain.Infrastructure;

public class OrderRepository : RepositoryBase<Order, Guid>
{
    public OrderRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await Query()
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }
}
```

### Domain Events

The `DomainDbContext` automatically publishes domain events when `SaveChangesAsync` is called:

```csharp
using Asm.Domain;

public class Order : Entity<Guid>
{
    public string OrderNumber { get; private set; }

    public Order(Guid id, string orderNumber) : base(id)
    {
        OrderNumber = orderNumber;
        AddDomainEvent(new OrderCreatedEvent(id, orderNumber));
    }
}

// Domain events are automatically published when SaveChangesAsync is called
await dbContext.Orders.Add(order);
await dbContext.SaveChangesAsync(); // Events published here
```

### DbSet Extensions

Use extension methods to enhance `DbSet` operations:

```csharp
using Asm.Domain.Infrastructure;

// Find an entity by ID
var order = await dbContext.Orders.FindByIdAsync(orderId);

// Check if an entity exists
bool exists = await dbContext.Orders.ExistsAsync(orderId);
```

### Specifications

Apply specifications to queries:

```csharp
using Asm.Domain;
using Asm.Domain.Infrastructure;

public class ActiveOrdersSpecification : ISpecification<Order>
{
    public IQueryable<Order> Apply(IQueryable<Order> query) =>
        query.Where(o => o.Status == OrderStatus.Active);
}

// Apply specification
var activeOrders = await dbContext.Orders
    .Apply(new ActiveOrdersSpecification())
    .ToListAsync();
```

## Dependencies

The `Asm.Domain.Infrastructure` library depends on the following packages:

- `Microsoft.EntityFrameworkCore`
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Instrumentation.EntityFrameworkCore`
- `OpenTelemetry.Instrumentation.SqlClient`
- `LazyCache`
- `LazyCache.AspNetCore`

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.