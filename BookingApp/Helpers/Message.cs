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
    }
}
