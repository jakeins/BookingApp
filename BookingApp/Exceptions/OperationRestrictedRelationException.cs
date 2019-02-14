using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class OperationRestrictedRelationException : OperationRestrictedException
    {
        public OperationRestrictedRelationException() : base()
        {
        }

        public OperationRestrictedRelationException(string message) : base(message)
        {
        }

        public OperationRestrictedRelationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OperationRestrictedRelationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}