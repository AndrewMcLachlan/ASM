# Asm.Testing.Domain

The `Asm.Testing.Domain` library provides specialized testing utilities for domain-driven design (DDD) applications. It includes mock implementations of Entity Framework Core components to facilitate unit testing of domain logic without requiring a real database.

## Features

- **MockDbSet**: A mock implementation of `DbSet<T>` for testing Entity Framework Core queries
- **MockDbSetFactory**: Factory methods for creating mock DbSets and queryables with any class type
- **TestAsyncQueryProvider**: Enables async LINQ queries against in-memory collections, including projections
- **TestAsyncEnumerable**: Provides async enumeration support for test data
- **Projection Support**: Full support for `.Select()` projections with async terminal operations
- **Moq Integration**: Works seamlessly with the Moq mocking framework
- **No Database Required**: Test domain logic and repositories without database dependencies

## Installation

To install the `Asm.Testing.Domain` library, use the .NET CLI:

`dotnet add package Asm.Testing.Domain`

Or via the NuGet Package Manager:

`Install-Package Asm.Testing.Domain`

## Usage

### Mocking DbSet for Unit Tests

Use `MockDbSet` to create testable `DbSet` instances for domain entities:

```csharp
using Asm.Testing.Domain;
using Moq;
using Microsoft.EntityFrameworkCore;

public class OrderRepositoryTests
{
    [Fact]
    public async Task GetActiveOrders_ReturnsOnlyActiveOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Active },
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Completed },
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Active }
        };

        var mockSet = new MockDbSet<Order>(orders);
        var mockContext = new Mock<MyDbContext>();
        mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

        var repository = new OrderRepository(mockContext.Object);

        // Act
        var activeOrders = await repository.GetActiveOrdersAsync();

        // Assert
        Assert.Equal(2, activeOrders.Count);
        Assert.All(activeOrders, o => Assert.Equal(OrderStatus.Active, o.Status));
    }
}
```

### Using MockDbSetFactory for Any Class Type

For entities that don't inherit from `Entity`, use `MockDbSetFactory`:

```csharp
using Asm.Testing.Domain;

[Fact]
public async Task GetInstitution_ReturnsCorrectInstitution()
{
    // Arrange - Institution is any class type
    var institutions = new List<Institution>
    {
        new Institution { Id = 1, Name = "Bank A" },
        new Institution { Id = 2, Name = "Bank B" }
    };

    var mockSet = MockDbSetFactory.Create(institutions);

    // Act
    var result = await mockSet.Object
        .Where(i => i.Id == 1)
        .SingleOrDefaultAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Bank A", result.Name);
}
```

### Testing CQRS Handlers with IQueryable

For CQRS handlers that receive `IQueryable<T>` as a dependency:

```csharp
using Asm.Testing.Domain;

public class GetInstitutionHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsProjectedModel()
    {
        // Arrange
        var entities = new List<InstitutionEntity>
        {
            new InstitutionEntity { Id = 1, Name = "Test Bank", Type = InstitutionType.Bank }
        };

        // Create an IQueryable that supports async operations
        var queryable = MockDbSetFactory.CreateQueryable(entities);
        var handler = new GetInstitutionHandler(queryable);

        // Act - handler uses .Where().Select(ToModel()).SingleOrDefaultAsync()
        var result = await handler.Handle(new GetInstitution(1), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Bank", result.Name);
    }
}
```

### Testing Projections with Select

The library fully supports projections with `.Select()` followed by async terminal operations:

```csharp
using Asm.Testing.Domain;

[Fact]
public async Task Query_WithProjection_WorksAsync()
{
    // Arrange
    var orders = new List<Order>
    {
        new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-001", Total = 100 },
        new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-002", Total = 200 }
    };

    var mockSet = MockDbSetFactory.Create(orders);

    // Act - project to DTO and use async operations
    var orderSummaries = await mockSet.Object
        .Where(o => o.Total > 50)
        .Select(o => new OrderSummaryDto { Number = o.OrderNumber, Total = o.Total })
        .ToListAsync();

    // Assert
    Assert.Equal(2, orderSummaries.Count);
}

[Fact]
public async Task Query_WithProjectionAndSingleOrDefault_WorksAsync()
{
    // Arrange
    var institutions = new List<Institution>
    {
        new Institution { Id = 1, Name = "Bank A" },
        new Institution { Id = 2, Name = "Bank B" }
    };

    var queryable = MockDbSetFactory.CreateQueryable(institutions);

    // Act - this pattern is common in CQRS handlers
    var result = await queryable
        .Where(i => i.Id == 1)
        .Select(i => new InstitutionModel { Id = i.Id, Name = i.Name })
        .SingleOrDefaultAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Bank A", result.Name);
}
```

### Mocking DbContext with Multiple DbSets

Create a complete mock context for testing:

```csharp
using Asm.Testing.Domain;
using Moq;

public class MyDbContextFixture
{
    public Mock<MyDbContext> MockContext { get; }
    public MockDbSet<Order> Orders { get; }
    public MockDbSet<Customer> Customers { get; }

    public MyDbContextFixture()
    {
        MockContext = new Mock<MyDbContext>();

        Orders = new MockDbSet<Order>(new List<Order>());
        Customers = new MockDbSet<Customer>(new List<Customer>());

        MockContext.Setup(c => c.Orders).Returns(Orders.Object);
        MockContext.Setup(c => c.Customers).Returns(Customers.Object);
    }
}
```

### Advanced: Using TestAsyncEnumerable Directly

For custom scenarios, you can use the async enumerable wrapper directly:

```csharp
using Asm.Testing.Domain;

var data = new List<Order> { /* test data */ };
var asyncEnumerable = new TestAsyncEnumerable<Order>(data);

// The enumerable supports async operations
await foreach (var order in asyncEnumerable)
{
    // Process order
}
```

## How It Works

The library solves a common problem when unit testing EF Core async operations:

1. **TestAsyncQueryProvider** compiles and executes LINQ expression trees directly, rather than delegating to the inner provider. This allows projections like `.Select(x => new Model()).SingleOrDefaultAsync()` to work correctly.

2. **TestAsyncEnumerable** overrides the `IQueryable.Provider` property to ensure chained LINQ operations continue using the async-capable provider throughout the query chain.

3. **MockDbSetFactory.CreateQueryable** provides a convenient way to create an `IQueryable<T>` that supports all EF Core async operations, perfect for testing CQRS handlers.

## Best Practices

1. **Use Fresh Data**: Create new test data for each test to avoid test interdependencies
2. **Mock SaveChanges**: Remember to mock `SaveChangesAsync` when testing persistence operations
3. **Verify Calls**: Use Moq's `Verify` to ensure methods are called as expected
4. **Use CreateQueryable for Handlers**: When testing CQRS handlers that receive `IQueryable<T>`, use `MockDbSetFactory.CreateQueryable()`
5. **Fixture Pattern**: Use test fixtures for complex context setups to share configuration

## Dependencies

The `Asm.Testing.Domain` library depends on the following packages:

- `Microsoft.EntityFrameworkCore`
- `Moq`
- `Asm.Domain`

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
