namespace Asm.Domain.Tests;

public class KeyedEntityEqualityTests
{
    /// <summary>
    /// Given a transient entity with a default key
    /// When it is compared to itself
    /// Then it is equal to itself
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EntityWithDefaultIdEqualsItself()
    {
        var entity = new TestKeyedEntity(0);

        // Reflexivity must hold even for a transient (default-key) entity.
        Assert.True(entity.Equals(entity));
        Assert.True(entity == entity);
    }

    /// <summary>
    /// Given two distinct transient entities each with a default key
    /// When they are compared
    /// Then they are not equal
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TwoDistinctTransientEntitiesAreNotEqual()
    {
        var a = new TestKeyedEntity(0);
        var b = new TestKeyedEntity(0);

        Assert.False(a.Equals(b));
    }

    /// <summary>
    /// Given entities of different types that share the same key
    /// When they are compared
    /// Then they are not equal in either direction
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void DifferentEntityTypesWithSameIdAreNotEqual()
    {
        var keyed = new TestKeyedEntity(1);
        var other = new OtherKeyedEntity(1);

        Assert.False(keyed.Equals(other));
        Assert.False(other.Equals(keyed));
    }

    /// <summary>
    /// Given two entities of the same type with the same key
    /// When they are compared
    /// Then they are equal and share a hash code
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void SameTypeSameIdAreEqual()
    {
        var a = new TestKeyedEntity(1);
        var b = new TestKeyedEntity(1);

        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    /// <summary>
    /// Given a transient entity added to a HashSet
    /// When the same instance is looked up and re-added
    /// Then it is found, not added twice, and the set contains a single item
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void HashSetTreatsSameInstanceAsPresent()
    {
        var entity = new TestKeyedEntity(0);
        var set = new HashSet<TestKeyedEntity> { entity };

        Assert.Contains(entity, set);
        Assert.False(set.Add(entity), "The same instance should not be added twice.");
        Assert.Single(set);
    }

    /// <summary>
    /// Given entities with duplicate keys added to a HashSet
    /// When the set is built
    /// Then entities sharing a key are deduplicated by key
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void HashSetDeduplicatesByKey()
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
