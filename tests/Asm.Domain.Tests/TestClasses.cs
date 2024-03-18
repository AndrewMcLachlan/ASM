namespace Asm.Domain.Tests;

internal class TestKeyedEntity(int id) : KeyedEntity<int>(id)
{
}

internal class TestNamedEntity(int id) : NamedEntity<int>(id)
{
}