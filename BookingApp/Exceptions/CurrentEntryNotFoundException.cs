using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class CurrentEntryNotFoundException : EntryNotFoundException
    {
        public CurrentEntryNotFoundException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        protected CurrentEntryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}