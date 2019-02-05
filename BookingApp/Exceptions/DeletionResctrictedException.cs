using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class DeletionResctrictedException : Exception
    {
        public DeletionResctrictedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeletionResctrictedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}