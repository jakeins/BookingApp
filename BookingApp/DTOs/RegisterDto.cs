using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RegisterDto : UserMinimalDto
    {
        public string UserName { get; set; }
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
