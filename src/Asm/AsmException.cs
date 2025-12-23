using System.Diagnostics.CodeAnalysis;

namespace Asm;

/// <summary>
/// Enhanced Exception class.
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public class AsmException : Exception
{
    #region Properties
    /// <summary>
    /// The unique ID of this exception.
    /// </summary>
    public Guid Id
    {
        get;
        protected set;
    }

    /// <summary>
    /// The error code for this exception.
    /// </summary>
    public int ErrorId
    {
        get;
        protected set;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="AsmException"/> class.
    /// </summary>
    public AsmException() : base()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsmException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public AsmException(string message) : base(message)
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsmException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
    public AsmException(string message, Exception innerException) : base(message, innerException)
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsmException"/> class.
    /// </summary>
    /// <param name="errorId">The ID of the error.</param>
    public AsmException(int errorId) : base()
    {
        Id = Guid.NewGuid();
        ErrorId = errorId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsmException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorId">The ID of the error.</param>
    public AsmException(string message, int errorId) : base(message)
    {
        Id = Guid.NewGuid();
        ErrorId = errorId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsmException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorId">The ID of the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
    public AsmException(string message, int errorId, Exception innerException) : base(message, innerException)
    {
        Id = Guid.NewGuid();
        ErrorId = errorId;
    }
    #endregion
}