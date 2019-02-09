using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class FieldValueTimeInvalidException : FieldValueException
    {
        public FieldValueTimeInvalidException()
        {
        }

        public FieldValueTimeInvalidException(string message) : base(message)
        {
        }

        public FieldValueTimeInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FieldValueTimeInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}