namespace Asm;

/// <summary>
/// Thrown when something is not found.
/// </summary>
public sealed class NotFoundException : ApplicationException
{
    /// <summary>
    /// Initializes a new insance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException() : base()
    {
    }

    /// <summary>
    /// Initializes a new insance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new insance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
