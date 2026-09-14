namespace Asm.Domain.SourceGenerators.Tests;

public class NavigationGeneratorTests
{
    private const string Entity = """
        using Asm.Domain;

        namespace Test.Entities;

        public class Institution
        {
            public string Name { get; set; } = "";
        }

        public partial class Account
        {
            [Navigation]
            public virtual partial Institution Institution { get; set; }
        }
        """;

    [Fact]
    [Trait("Category", "Unit")]
    public void Generates_An_Implementation_That_Compiles()
    {
        var run = GeneratorHarness.Run(Entity);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.Contains("partial Test.Entities.Institution Institution", run.GeneratedSource.Replace("global::", String.Empty));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Reading_An_Unloaded_Navigation_Throws()
    {
        var account = Activator.CreateInstance(GeneratorHarness.Run(Entity).Emit().GetType("Test.Entities.Account")!)!;

        var exception = Assert.Throws<TargetInvocationException>(() =>
            account.GetType().GetProperty("Institution")!.GetValue(account));

        var inner = Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Equal("Navigation property 'Account.Institution' has not been loaded. Include it in the query that loaded this entity.", inner.Message);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void A_Loaded_Navigation_Round_Trips()
    {
        var assembly = GeneratorHarness.Run(Entity).Emit();
        var account = Activator.CreateInstance(assembly.GetType("Test.Entities.Account")!)!;
        var institution = Activator.CreateInstance(assembly.GetType("Test.Entities.Institution")!)!;

        var property = account.GetType().GetProperty("Institution")!;
        property.SetValue(account, institution);

        Assert.Same(institution, property.GetValue(account));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void The_Generated_Property_Stays_Virtual_For_Lazy_Loading_Proxies()
    {
        var assembly = GeneratorHarness.Run(Entity).Emit();

        Assert.True(assembly.GetType("Test.Entities.Account")!.GetProperty("Institution")!.GetGetMethod()!.IsVirtual);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void The_Backing_Field_Follows_The_Convention_Entity_Framework_Binds_To()
    {
        var assembly = GeneratorHarness.Run(Entity).Emit();

        Assert.NotNull(assembly.GetType("Test.Entities.Account")!
            .GetField("<Institution>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Handles_Nested_And_Generic_Containing_Types()
    {
        var run = GeneratorHarness.Run("""
            using Asm.Domain;

            namespace Test.Entities;

            public class Institution;

            public partial class Outer<T>
            {
                public partial class Inner
                {
                    [Navigation]
                    public partial Institution Institution { get; set; }
                }
            }
            """);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.Contains("partial class Outer<T>", run.GeneratedSource);
        Assert.Contains("partial class Inner", run.GeneratedSource);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Handles_The_Global_Namespace()
    {
        var run = GeneratorHarness.Run("""
            using Asm.Domain;

            public class Institution;

            public partial class Account
            {
                [Navigation]
                public partial Institution Institution { get; set; }
            }
            """);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.DoesNotContain("namespace", run.GeneratedSource);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("ASM1001", "public virtual Institution Institution { get; set; }")]
    [InlineData("ASM1003", "public virtual partial Institution? Institution { get; set; }")]
    [InlineData("ASM1003", "public virtual partial int Institution { get; set; }")]
    [InlineData("ASM1004", "public virtual partial Institution Institution { get; }")]
    [InlineData("ASM1006", "public virtual partial Institution Institution { set; }")]
    public void Reports(string expected, string property)
    {
        var run = GeneratorHarness.Run($$"""
            using Asm.Domain;

            namespace Test.Entities;

            public class Institution;

            public partial class Account
            {
                [Navigation]
                {{property}}
            }
            """);

        Assert.Equal([expected], run.DiagnosticIds);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Reports_A_Containing_Type_That_Is_Not_Partial()
    {
        var run = GeneratorHarness.Run("""
            using Asm.Domain;

            namespace Test.Entities;

            public class Institution;

            public class Outer
            {
                public partial class Account
                {
                    [Navigation]
                    public partial Institution Institution { get; set; }
                }
            }
            """);

        Assert.Equal(["ASM1002"], run.DiagnosticIds);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Reports_A_Hand_Written_Implementation()
    {
        var run = GeneratorHarness.Run("""
            using Asm.Domain;

            namespace Test.Entities;

            public class Institution;

            public partial class Account
            {
                [Navigation]
                public partial Institution Institution { get; set; }
            }

            public partial class Account
            {
                public partial Institution Institution { get => null!; set { } }
            }
            """);

        Assert.Equal(["ASM1005"], run.DiagnosticIds);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Carries_Type_Parameter_Constraints_Onto_The_Generated_Declaration()
    {
        var run = GeneratorHarness.Run("""
            using Asm.Domain;

            namespace Test.Entities;

            public class Institution;

            public partial class Account<TKey, TValue>
                where TKey : class, System.IComparable<TKey>, new()
                where TValue : struct
            {
                [Navigation]
                public virtual partial Institution Institution { get; set; }
            }
            """);

        // Without the where clauses the compiler reports CS0265 for inconsistent constraints.
        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);

        var generated = run.GeneratedSource.Replace("global::", String.Empty);
        Assert.Contains("where TKey : class, System.IComparable<TKey>, new()", generated);
        Assert.Contains("where TValue : struct", generated);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Distinguishes_Entities_Whose_Names_Flatten_To_The_Same_Hint()
    {
        // Account<T> and Account_T_ both flatten to "Account_T_". Duplicate hint names fail the
        // build, so this is only green while the hint carries something that tells them apart.
        var run = GeneratorHarness.Run("""
            using Asm.Domain;

            namespace Test.Entities;

            public class Institution;

            public partial class Account<T>
            {
                [Navigation]
                public virtual partial Institution Institution { get; set; }
            }

            public partial class Account_T_
            {
                [Navigation]
                public virtual partial Institution Institution { get; set; }
            }
            """);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);

        var generated = run.GeneratedSource;
        Assert.Contains("partial class Account<T>", generated);
        Assert.Contains("partial class Account_T_", generated);
    }
}
