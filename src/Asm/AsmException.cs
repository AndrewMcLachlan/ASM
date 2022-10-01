using System.Runtime.Serialization;

namespace Asm;

/// <summary>
/// Enhanced Exception class.
/// </summary>
[Serializable]
public class AsmException : ApplicationException
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AsmException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected AsmException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        object? idObj = info.GetValue("Id", typeof(Guid));

        Id = idObj == null ? new Guid() : (Guid)idObj;

        ErrorId = info.GetInt32("ErrorId");
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Sets the <see cref="SerializationInfo"/>  with information about the exception.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null) throw new ArgumentNullException(nameof(info));

        base.GetObjectData(info, context);

        info.AddValue("Id", Id);
        info.AddValue("ErrorId", ErrorId);
    }
    #endregion
}