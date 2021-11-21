using System;
using System.Runtime.Serialization;

namespace Asm.Domain
{
    public class ExistsException : ApplicationException
    {
        public ExistsException()
        {
        }

        public ExistsException(string? message) : base(message)
        {
        }

        public ExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
