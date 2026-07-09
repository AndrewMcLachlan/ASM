namespace Asm.Domain.Tests;

public class KeyedEntityEqualityTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Entity_WithDefaultId_EqualsItself()
    {
        var entity = new TestKeyedEntity(0);

        // Reflexivity must hold even for a transient (default-key) entity.
        Assert.True(entity.Equals(entity));
        Assert.True(entity == entity);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void TwoDistinctTransientEntities_AreNotEqual()
    {
        var a = new TestKeyedEntity(0);
        var b = new TestKeyedEntity(0);

        Assert.False(a.Equals(b));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DifferentEntityTypes_WithSameId_AreNotEqual()
    {
        var keyed = new TestKeyedEntity(1);
        var other = new OtherKeyedEntity(1);

        Assert.False(keyed.Equals(other));
        Assert.False(other.Equals(keyed));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SameTypeSameId_AreEqual()
    {
        var a = new TestKeyedEntity(1);
        var b = new TestKeyedEntity(1);

        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void HashSet_TreatsSameInstanceAsPresent()
    {
        var entity = new TestKeyedEntity(0);
        var set = new HashSet<TestKeyedEntity> { entity };

        Assert.Contains(entity, set);
        Assert.False(set.Add(entity), "The same instance should not be added twice.");
        Assert.Single(set);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void HashSet_DeduplicatesByKey()
    {
        var set = new HashSet<TestKeyedEntity>
        {
            new(1),
            new(1),
            new(2),
        };

        Assert.Equal(2, set.Count);
    }
}
