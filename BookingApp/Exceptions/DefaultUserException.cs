using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Exceptions
{
    public class DefaultUserException : Exception
    {
        public DefaultUserException(string message) : base(message)
        {

        }
    }
}
