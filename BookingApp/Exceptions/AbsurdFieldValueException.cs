using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class AbsurdFieldValueException : FieldValueException
    {
        public AbsurdFieldValueException(string message) : base(message)
        {
        }

        protected AbsurdFieldValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}