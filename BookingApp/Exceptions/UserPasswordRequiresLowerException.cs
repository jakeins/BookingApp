using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserPasswordRequiresLowerException : UserPasswordException
    {
        public UserPasswordRequiresLowerException() : base()
        {
        }

        public UserPasswordRequiresLowerException(string message) : base(message)
        {
        }

        public UserPasswordRequiresLowerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserPasswordRequiresLowerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}