using System.Collections.Immutable;
using System.Reflection;
using Asm.Domain;
using Asm.Domain.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Asm.Domain.SourceGenerators.Tests;

internal sealed record GeneratorRun(Compilation Compilation, ImmutableArray<Diagnostic> GeneratorDiagnostics)
{
    public IEnumerable<string> DiagnosticIds => GeneratorDiagnostics.Select(diagnostic => diagnostic.Id);

    public string GeneratedSource =>
        String.Concat(Compilation.SyntaxTrees.Skip(1).Select(tree => tree.ToString()));

    public IEnumerable<Diagnostic> CompilationErrors =>
        Compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Emits the generated code and loads it, so a test can exercise the behaviour rather than the text.
    /// </summary>
    public Assembly Emit()
    {
        using var stream = new MemoryStream();
        var result = Compilation.Emit(stream);

        Assert.True(result.Success, String.Join(Environment.NewLine, result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));

        return Assembly.Load(stream.ToArray());
    }
}

internal static class GeneratorHarness
{
    private static readonly ImmutableArray<MetadataReference> References = BuildReferences();

    public static GeneratorRun Run(string source)
    {
        var compilation = CSharpCompilation.Create(
            $"Generated_{Guid.NewGuid():N}",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        CSharpGeneratorDriver.Create(new NavigationGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var updated, out var diagnostics);

        return new GeneratorRun(updated, diagnostics);
    }

    private static ImmutableArray<MetadataReference> BuildReferences()
    {
        var platform = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? String.Empty;

        return
        [
            .. platform.Split(Path.PathSeparator)
                       .Where(path => path.Length > 0)
                       .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path)),
            MetadataReference.CreateFromFile(typeof(NavigationAttribute).Assembly.Location),
        ];
    }
}
