using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class DeleteResctrictedException : DeleteException
    {
        public DeleteResctrictedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeleteResctrictedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}