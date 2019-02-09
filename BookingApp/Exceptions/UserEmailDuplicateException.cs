using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UserEmailDuplicateException : UserEmailException
    {
        public UserEmailDuplicateException() : base()
        {
        }

        public UserEmailDuplicateException(string message) : base(message)
        {
        }

        public UserEmailDuplicateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserEmailDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}