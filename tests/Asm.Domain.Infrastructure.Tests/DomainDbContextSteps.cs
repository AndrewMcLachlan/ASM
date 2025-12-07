using System.ComponentModel.DataAnnotations;
using Asm.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class DomainDbContextSteps
{
    private TestDomainDbContext _context = null!;
    private Mock<IPublisher> _mockPublisher = null!;
    private int _result;

    #region Test Types

    public class TestDomainDbContext(DbContextOptions options, IPublisher publisher) : DomainDbContext(options, publisher)
    {
        public DbSet<DomainDbContextTestEntity> TestEntities { get; set; } = null!;
    }

    public class DomainDbContextTestEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        public ICollection<IDomainEvent> Events { get; } = [];
    }

    public class DomainDbContextTestDomainEvent : IDomainEvent
    {
    }

    #endregion

    [Given(@"I have a DomainDbContext with a mock publisher")]
    public void GivenIHaveADomainDbContextWithAMockPublisher()
    {
        _mockPublisher = new Mock<IPublisher>();
        _mockPublisher
            .Setup(p => p.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var options = new DbContextOptionsBuilder<TestDomainDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDomainDbContext(options, _mockPublisher.Object);
    }

    [When(@"I call SaveChangesAsync with acceptAllChangesOnSuccess true")]
    public async Task WhenICallSaveChangesAsyncWithAcceptAllChangesOnSuccessTrue()
    {
        _result = await _context.SaveChangesAsync(true);
    }

    [When(@"I call SaveChangesAsync with acceptAllChangesOnSuccess false")]
    public async Task WhenICallSaveChangesAsyncWithAcceptAllChangesOnSuccessFalse()
    {
        _result = await _context.SaveChangesAsync(false);
    }

    [When(@"I call SaveChangesAsync with default cancellation token")]
    public async Task WhenICallSaveChangesAsyncWithDefaultCancellationToken()
    {
        _result = await _context.SaveChangesAsync(CancellationToken.None);
    }

    [When(@"I call SaveChanges on the DomainDbContext")]
    public void WhenICallSaveChangesOnTheDomainDbContext()
    {
        _result = _context.SaveChanges();
    }

    [Then(@"the result should be (.*)")]
    public void ThenTheResultShouldBe(int expected)
    {
        Assert.Equal(expected, _result);
    }

    [Then(@"Publish should not have been called")]
    public void ThenPublishShouldNotHaveBeenCalled()
    {
        _mockPublisher.Verify(
            p => p.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [AfterScenario]
    public void Cleanup()
    {
        _context?.Dispose();
    }
}
