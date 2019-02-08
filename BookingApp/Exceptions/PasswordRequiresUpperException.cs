using System;

namespace BookingApp.Exceptions
{
    public class PasswordRequiresUpperException : Exception
    {
        public PasswordRequiresUpperException(string message) : base(message)
        {

        }
    }
}
