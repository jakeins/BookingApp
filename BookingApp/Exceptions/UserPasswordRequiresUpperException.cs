using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserPasswordRequiresUpperException : UserPasswordException
    {
        public UserPasswordRequiresUpperException() : base()
        {
        }

        public UserPasswordRequiresUpperException(string message) : base(message)
        {
        }

        public UserPasswordRequiresUpperException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserPasswordRequiresUpperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}