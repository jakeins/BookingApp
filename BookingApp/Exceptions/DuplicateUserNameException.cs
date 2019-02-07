using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Exceptions
{
    public class DuplicateUserNameException : Exception
    {
        public DuplicateUserNameException(string message) : base(message)
        {

        }
    }
}
