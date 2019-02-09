using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserEmailInvalidException : UserEmailException
    {
        public UserEmailInvalidException() : base()
        {
        }

        public UserEmailInvalidException(string message) : base(message)
        {
        }

        public UserEmailInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserEmailInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}