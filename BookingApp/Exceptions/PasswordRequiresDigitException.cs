using System;

namespace BookingApp.Exceptions
{
    public class PasswordRequiresDigitException : Exception
    {
        public PasswordRequiresDigitException(string message) : base(message)
        {

        }
    }
}
