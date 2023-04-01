namespace Asm.Domain.Tests;

class TestKeyedEntity : KeyedEntity<int>
{
    public TestKeyedEntity(int id) : base(id) { }
}


class TestNamedEntity : NamedEntity<int>
{
    public TestNamedEntity(int id) : base(id) { }
}