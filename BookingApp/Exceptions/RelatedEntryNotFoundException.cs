using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
    [Serializable]
    public class RelatedEntryNotFoundException : EntryNotFoundException
    {
        public RelatedEntryNotFoundException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        protected RelatedEntryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}