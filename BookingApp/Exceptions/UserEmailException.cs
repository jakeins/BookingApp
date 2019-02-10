using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserEmailException : UserException
    {
        public UserEmailException() : base()
        {
        }

        public UserEmailException(string message) : base(message)
        {
        }

        public UserEmailException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserEmailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}