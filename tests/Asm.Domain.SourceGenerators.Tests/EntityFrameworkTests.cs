using Asm.Domain;
using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.SourceGenerators.Tests;

/// <summary>
/// Exercises a generated navigation through a real Entity Framework Core model, rather than trusting
/// that the backing field is discovered by convention.
/// </summary>
public class EntityFrameworkTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Materialisation_Populates_The_Navigation_Without_Reading_The_Getter()
    {
        var database = Guid.NewGuid().ToString();
        await using var context = new OrdersContext(database);

        context.Customers.Add(new Customer { Id = 1, Name = "Acme" });
        context.Orders.Add(new Order { Id = 1, CustomerId = 1 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var reading = new OrdersContext(database);
        var order = await reading.Orders.Include(o => o.Customer).SingleAsync(TestContext.Current.CancellationToken);

        Assert.Equal("Acme", order.Customer.Name);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void The_Model_Binds_The_Navigation_To_The_Generated_Backing_Field()
    {
        var database = Guid.NewGuid().ToString();
        using var context = new OrdersContext(database);

        var navigation = context.Model.FindEntityType(typeof(Order))!.FindNavigation(nameof(Order.Customer))!;

        Assert.Equal("<Customer>k__BackingField", navigation.FieldInfo?.Name);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task An_Unloaded_Navigation_Throws_Rather_Than_Returning_Null()
    {
        var database = Guid.NewGuid().ToString();
        await using var context = new OrdersContext(database);

        context.Customers.Add(new Customer { Id = 1, Name = "Acme" });
        context.Orders.Add(new Order { Id = 1, CustomerId = 1 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var reading = new OrdersContext(database);
        var order = await reading.Orders.SingleAsync(TestContext.Current.CancellationToken);

        Assert.Throws<InvalidOperationException>(() => order.Customer);
    }

    private sealed class OrdersContext(string name) : DbContext
    {

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseInMemoryDatabase(name);
    }
}

internal sealed class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
}

internal partial class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [Navigation]
    public virtual partial Customer Customer { get; set; }
}
