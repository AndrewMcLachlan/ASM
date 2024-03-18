using System.Security;

namespace Asm;

/// <summary>
/// Thrown when a user is not authorised to perform an action.
/// </summary>
public class NotAuthorisedException : SecurityException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotAuthorisedException"/> class.
    /// </summary>
    public NotAuthorisedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotAuthorisedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotAuthorisedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotAuthorisedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <c>inner</c> parameter is not <c>null</c>, the current exception is raised in a <c>catch</c> block that handles the inner exception.</param>
    public NotAuthorisedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotAuthorisedException"/> class with a specified error message and the permission type that caused the exception to be thrown.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="type">The type of the permission that caused the exception to be thrown.</param>
    public NotAuthorisedException(string message, Type type) : base(message, type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotAuthorisedException"/> class with a specified error message, the permission type that caused the exception to be thrown, and the permission state.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="type">The type of the permission that caused the exception to be thrown.</param>
    /// <param name="state">The state of the permission that caused the exception to be thrown.</param>
    public NotAuthorisedException(string message, Type type, string state) : base(message, type, state)
    {
    }
}
