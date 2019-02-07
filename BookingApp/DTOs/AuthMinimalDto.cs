using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class AuthMinimalDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
