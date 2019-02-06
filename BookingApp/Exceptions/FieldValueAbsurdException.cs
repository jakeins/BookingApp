using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class FieldValueAbsurdException : FieldValueException
    {
        public FieldValueAbsurdException(string message) : base(message)
        {
        }

        protected FieldValueAbsurdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}