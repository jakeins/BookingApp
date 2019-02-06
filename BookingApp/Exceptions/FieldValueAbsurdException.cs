using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class FieldValueAbsurdException : FieldValueException
    {
        public FieldValueAbsurdException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        protected FieldValueAbsurdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}