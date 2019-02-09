using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserPasswordRequiresNonAlphanumericException : UserPasswordException
    {
        public UserPasswordRequiresNonAlphanumericException() : base()
        {
        }

        public UserPasswordRequiresNonAlphanumericException(string message) : base(message)
        {
        }

        public UserPasswordRequiresNonAlphanumericException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserPasswordRequiresNonAlphanumericException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}