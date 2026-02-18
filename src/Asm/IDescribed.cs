namespace Asm;

/// <summary>
/// Represents an object with a description.
/// </summary>
public interface IDescribed
{
    /// <summary>
    /// Gets the description of the object.
    /// </summary>
    string? Description { get; init; }
}
