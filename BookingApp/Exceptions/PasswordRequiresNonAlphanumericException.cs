using System;


namespace BookingApp.Exceptions
{
    public class PasswordRequiresNonAlphanumericException : Exception
    {
        public PasswordRequiresNonAlphanumericException(string message) : base(message)
        {

        }
    }
}
