using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class UserPasswordChangeDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
