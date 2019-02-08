using System;

namespace BookingApp.Exceptions
{
    public class PasswordRequiresLowerException : Exception
    {
        public PasswordRequiresLowerException(string message) : base(message)
        {

        }
    }
}
