namespace Asm.AspNetCore;

/// <summary>
/// Options for binding command parameters.
/// </summary>
public enum CommandBinding
{
    /// <summary>
    /// No parameter binding specified.
    /// </summary>
    /// <remarks>
    /// Unless a BindAsync or TryParse method is defined, body binding will be used.
    /// </remarks>
    None = 0,
    /// <summary>
    /// Use body binding with the <see cref="Microsoft.AspNetCore.Mvc.FromBodyAttribute"/>.
    /// </summary>
    Body = 1,
    /// <summary>
    /// Use parameter binding with the <see cref="AsParametersAttribute"/>.
    /// </summary>
    Parameters = 2,
}
