; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 5.1

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
ASM1001 | Asm.Domain | Error | Navigation property must be partial
ASM1002 | Asm.Domain | Error | Type containing a navigation property must be partial
ASM1003 | Asm.Domain | Error | Navigation property must be a non-nullable reference type
ASM1004 | Asm.Domain | Error | Navigation property must have a setter
ASM1005 | Asm.Domain | Error | Navigation property already has an implementation
