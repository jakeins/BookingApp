using System;

namespace BookingApp.Exceptions
{
    public class PasswordTooShortException : Exception
    {
        public PasswordTooShortException(string message) : base(message)
        {

        }
    }
}
