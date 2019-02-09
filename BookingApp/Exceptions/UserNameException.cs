using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserNameException : UserException
    {
        public UserNameException() : base()
        {
        }

        public UserNameException(string message) : base(message)
        {
        }

        public UserNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}