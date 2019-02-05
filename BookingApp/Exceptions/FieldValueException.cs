using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class FieldValueException : Exception
    {
        public FieldValueException(string message) : base(message)
        {
        }

        protected FieldValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}