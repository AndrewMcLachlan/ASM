using Microsoft.CodeAnalysis;

namespace Asm.Domain.SourceGenerators;

internal static class Diagnostics
{
    private const string Category = "Asm.Domain";

    public static readonly DiagnosticDescriptor PropertyMustBePartial = new(
        "ASM1001",
        "Navigation property must be partial",
        "Navigation property '{0}' must be declared partial so its implementation can be generated",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ContainingTypeMustBePartial = new(
        "ASM1002",
        "Type containing a navigation property must be partial",
        "Type '{0}' must be declared partial because it contains the navigation property '{1}'",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PropertyMustBeNonNullableReference = new(
        "ASM1003",
        "Navigation property must be a non-nullable reference type",
        "Navigation property '{0}' must be a non-nullable reference type; an optional navigation is already honest about being absent",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PropertyMustHaveSetter = new(
        "ASM1004",
        "Navigation property must have a setter",
        "Navigation property '{0}' must have a setter so the data store can populate it",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PropertyAlreadyImplemented = new(
        "ASM1005",
        "Navigation property already has an implementation",
        "Navigation property '{0}' already has a hand-written implementation; remove it or remove the attribute",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
