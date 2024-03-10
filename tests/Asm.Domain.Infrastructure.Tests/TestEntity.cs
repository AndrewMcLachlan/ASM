using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain.Infrastructure.Tests;

[Table("TestEntity")]
internal class TestEntity([DisallowNull] int id) : KeyedEntity<int>(id)
{
    public TestEntity() : this(0) { }

    [Key]
    public int TestKey { get; set; }

    public void TriggerDomainEvent()
    {
        this.Events.Add(new TestDomainEvent());
    }
}
