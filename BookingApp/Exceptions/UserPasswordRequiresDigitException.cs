using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserPasswordRequiresDigitException : UserPasswordException
    {
        public UserPasswordRequiresDigitException() : base()
        {
        }

        public UserPasswordRequiresDigitException(string message) : base(message)
        {
        }

        public UserPasswordRequiresDigitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserPasswordRequiresDigitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}