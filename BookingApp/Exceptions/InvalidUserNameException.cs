using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Exceptions
{
    public class InvalidUserNameException : Exception
    {
        public InvalidUserNameException(string message) : base(message)
        {

        }
    }
}
