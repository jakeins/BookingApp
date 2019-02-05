using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UpdateFailedException : Exception
    {
        public UpdateFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UpdateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}