namespace Asm;

/// <summary>
/// An item already exists exception.
/// </summary>
public sealed class ExistsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExistsException"/> class.
    /// </summary>
    public ExistsException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExistsException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message string.</param>
    public ExistsException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExistsException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message string.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public ExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
