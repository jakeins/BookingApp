using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserPasswordException : UserException
    {
        public UserPasswordException() : base()
        {
        }

        public UserPasswordException(string message) : base(message)
        {
        }

        public UserPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}