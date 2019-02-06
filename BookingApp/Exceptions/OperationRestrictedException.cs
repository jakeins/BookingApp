using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class OperationRestrictedException : OperationFailedException
    {
        public OperationRestrictedException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        protected OperationRestrictedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}