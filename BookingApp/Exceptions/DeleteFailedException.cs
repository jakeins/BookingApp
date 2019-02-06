using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class DeleteFailedException : DeleteException
    {
        public DeleteFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeleteFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}