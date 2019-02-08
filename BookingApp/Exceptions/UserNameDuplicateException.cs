using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    public class UserNameDuplicateException : UserNameException
    {
        public UserNameDuplicateException() : base()
        {
        }

        public UserNameDuplicateException(string message) : base(message)
        {
        }

        public UserNameDuplicateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNameDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}