using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class UserUpdateDTO :UserMinimalDto
    {
        public bool EmailConfirmed { get; set; }
    }
}
