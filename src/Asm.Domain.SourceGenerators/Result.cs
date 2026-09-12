using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Asm.Domain.SourceGenerators;

/// <summary>
/// The outcome of analysing one <c>[Navigation]</c> property: either source to add or a diagnostic.
/// </summary>
/// <remarks>
/// Everything held here must compare by value, or the incremental pipeline re-runs downstream steps
/// on every keystroke.
/// </remarks>
internal readonly record struct Result(string? HintName, string? Source, DiagnosticInfo? Diagnostic)
{
    public static Result Generated(string hintName, string source) => new(hintName, source, null);

    public static Result Failed(DiagnosticDescriptor descriptor, SyntaxToken token, params string[] messageArguments) =>
        new(null, null, DiagnosticInfo.Create(descriptor, token, messageArguments));
}

internal readonly record struct DiagnosticInfo(DiagnosticDescriptor Descriptor, LocationInfo? Location, string Arguments)
{
    private const char Separator = '\u001f';

    public static DiagnosticInfo Create(DiagnosticDescriptor descriptor, SyntaxToken token, params string[] messageArguments) =>
        new(descriptor, LocationInfo.From(token), string.Join(Separator.ToString(), messageArguments));

    public Diagnostic ToDiagnostic() =>
        Diagnostic.Create(Descriptor, Location?.ToLocation(), Arguments.Split(Separator));
}

internal readonly record struct LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);

    public static LocationInfo? From(SyntaxToken token)
    {
        var location = token.GetLocation();

        return location.SourceTree is null
            ? null
            : new LocationInfo(location.SourceTree.FilePath, location.SourceSpan, location.GetLineSpan().Span);
    }
}
