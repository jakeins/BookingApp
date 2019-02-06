using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class EntryNotFoundException : OperationFailedException
    {
        public EntryNotFoundException() : base()
        {
        }

        public EntryNotFoundException(string message) : base(message)
        {
        }

        public EntryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}