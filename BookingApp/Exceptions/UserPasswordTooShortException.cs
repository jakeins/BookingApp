using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserPasswordTooShortException : UserPasswordException
    {
        public UserPasswordTooShortException() : base()
        {
        }

        public UserPasswordTooShortException(string message) : base(message)
        {
        }

        public UserPasswordTooShortException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserPasswordTooShortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}