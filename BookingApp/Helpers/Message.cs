using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Helpers
{
    public class Message
    {
        public virtual string Destination { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }

        public override bool Equals(object obj)
        {
            var message = obj as Message;
            return message != null &&
                   Destination == message.Destination &&
                   Subject == message.Subject &&
                   Body == message.Body;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Destination, Subject, Body);
        }
    }
}
