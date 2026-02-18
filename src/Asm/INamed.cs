namespace Asm;

/// <summary>
/// Represents an object with a name.
/// </summary>
public interface INamed
{
    /// <summary>
    /// Gets the name of the object.
    /// </summary>
    string Name { get; init; }
}
