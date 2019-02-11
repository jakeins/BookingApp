using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class UserUpdateDTO :UserMinimalDto
    {
        public string Id { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
