using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class FieldValueException : StorageException
    {
        public FieldValueException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        protected FieldValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}