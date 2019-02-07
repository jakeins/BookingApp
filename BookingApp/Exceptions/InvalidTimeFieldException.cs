using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BookingApp.Exceptions
{
    public class InvalidTimeFieldException : FieldValueException
    {
        public InvalidTimeFieldException()
        {
        }

        public InvalidTimeFieldException(string message) : base(message)
        {
        }

        public InvalidTimeFieldException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTimeFieldException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
