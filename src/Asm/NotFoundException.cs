using System.Diagnostics.CodeAnalysis;

namespace Asm;

/// <summary>
/// Thrown when something is not found.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message string.</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message string.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
