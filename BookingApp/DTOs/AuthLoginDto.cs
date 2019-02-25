using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class AuthLoginDto : AuthMinimalDto
    {
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }
    }
}
