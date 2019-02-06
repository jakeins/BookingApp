using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class UpdateRestrictedException : UpdateException
    {
        public UpdateRestrictedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UpdateRestrictedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}