using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class OperationFailedException : StorageException
    {
        public OperationFailedException() : base()
        {
        }

        public OperationFailedException(string message) : base(message)
        {
        }

        public OperationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}