# Asm.Testing.Domain

The `Asm.Testing.Domain` library provides specialized testing utilities for domain-driven design (DDD) applications. It includes mock implementations of Entity Framework Core components to facilitate unit testing of domain logic without requiring a real database.

## Features

- **MockDbSet**: A mock implementation of `DbSet<T>` for testing Entity Framework Core queries
- **TestAsyncQueryProvider**: Enables async LINQ queries against in-memory collections
- **TestAsyncEnumerable**: Provides async enumeration support for test data
- **Moq Integration**: Works seamlessly with the Moq mocking framework
- **No Database Required**: Test domain logic and repositories without database dependencies

## Installation

To install the `Asm.Testing.Domain` library, use the .NET CLI:

`dotnet add package Asm.Testing.Domain`

Or via the NuGet Package Manager:

`Install-Package Asm.Testing.Domain`

## Usage

### Mocking DbSet for Unit Tests

Use `MockDbSet` to create testable `DbSet` instances:

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

### Testing Async LINQ Queries

The `MockDbSet` supports async LINQ operations:

```csharp
using Asm.Testing.Domain;

[Fact]
public async Task FindById_ReturnsCorrectOrder()
{
    // Arrange
    var orderId = Guid.NewGuid();
    var orders = new List<Order>
    {
        new Order { Id = orderId, OrderNumber = "ORD-001" },
        new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-002" }
    };

    var mockSet = new MockDbSet<Order>(orders);

    // Act
    var order = await mockSet.Object
        .FirstOrDefaultAsync(o => o.Id == orderId);

    // Assert
    Assert.NotNull(order);
    Assert.Equal("ORD-001", order.OrderNumber);
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

[Collection("Database")]
public class OrderServiceTests : IClassFixture<MyDbContextFixture>
{
    private readonly MyDbContextFixture _fixture;

    public OrderServiceTests(MyDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateOrder_AddsToDatabase()
    {
        // Arrange
        var service = new OrderService(_fixture.MockContext.Object);
        var customer = new Customer { Id = Guid.NewGuid(), Name = "John Doe" };
        _fixture.Customers.Add(customer);

        // Act
        var order = await service.CreateOrderAsync(customer.Id, "ORD-001");

        // Assert
        Assert.NotNull(order);
        Assert.Single(_fixture.Orders);
    }
}
```

### Testing Repository Methods

Test repository implementations without a database:

```csharp
using Asm.Testing.Domain;
using Asm.Domain.Infrastructure;

[Fact]
public async Task GetBySpecification_AppliesCorrectFilter()
{
    // Arrange
    var orders = new List<Order>
    {
        new Order { Id = Guid.NewGuid(), Status = OrderStatus.Active, Total = 100 },
        new Order { Id = Guid.NewGuid(), Status = OrderStatus.Active, Total = 50 },
        new Order { Id = Guid.NewGuid(), Status = OrderStatus.Completed, Total = 200 }
    };

    var mockSet = new MockDbSet<Order>(orders);
    var mockContext = new Mock<MyDbContext>();
    mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

    var repository = new OrderRepository(mockContext.Object);

    // Act
    var result = await repository.Query()
        .Where(o => o.Status == OrderStatus.Active && o.Total > 75)
        .ToListAsync();

    // Assert
    Assert.Single(result);
    Assert.Equal(100, result[0].Total);
}
```

### Advanced: Custom Query Provider

For advanced scenarios, you can work directly with the test query provider:

```csharp
using Asm.Testing.Domain;

var data = new List<Order> { /* test data */ };
var queryProvider = new TestAsyncQueryProvider<Order>(data.AsQueryable());
var enumerable = new TestAsyncEnumerable<Order>(data.AsQueryable());

// Use in custom mock setups
mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(queryProvider);
mockSet.As<IAsyncEnumerable<Order>>().Setup(m => m.GetAsyncEnumerator(default))
    .Returns(enumerable.GetAsyncEnumerator);
```

## Best Practices

1. **Use Fresh Data**: Create new test data for each test to avoid test interdependencies
2. **Mock SaveChanges**: Remember to mock `SaveChangesAsync` when testing persistence operations
3. **Verify Calls**: Use Moq's `Verify` to ensure methods are called as expected
4. **Fixture Pattern**: Use test fixtures for complex context setups to share configuration

## Dependencies

The `Asm.Testing.Domain` library depends on the following packages:

- `Microsoft.EntityFrameworkCore`
- `Moq`
- `Asm.Domain`

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.