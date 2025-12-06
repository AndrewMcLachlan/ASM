using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class DbSetExtensionsSteps(ScenarioContext context)
{
    private DbSet<DbSetTestEntity> _dbSet = null!;

    #region Test Types

    public class DbSetTestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }

    #endregion

    [Given(@"I have a null DbSet")]
    public void GivenIHaveANullDbSet()
    {
        _dbSet = null!;
    }

    [When(@"I call FindAsync on the null DbSet")]
    public Task WhenICallFindAsyncOnTheNullDbSet()
    {
        return context.CatchExceptionAsync(async () => await DbSetExtensions.FindAsync(_dbSet, 1));
    }
}
