using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    public class UserNameInvalidException : UserNameException
    {
        public UserNameInvalidException() : base()
        {
        }

        public UserNameInvalidException(string message) : base(message)
        {
        }

        public UserNameInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNameInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}