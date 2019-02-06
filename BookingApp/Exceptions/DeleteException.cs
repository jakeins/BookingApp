using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class DeleteException : Exception
    {
        public DeleteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeleteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}