using System;
using System.Runtime.Serialization;

namespace BookingApp.Exceptions
{
        [Serializable]
        public class NotIssetTreeGroupException : ApplicationException
    {
            public NotIssetTreeGroupException() : base()
            {
            }

            public NotIssetTreeGroupException(string message) : base(message)
            {
            }

            public NotIssetTreeGroupException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected NotIssetTreeGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
}
