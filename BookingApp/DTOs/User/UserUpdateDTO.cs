using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class UserUpdateDTO
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool? ApprovalStatus { get; set; }
        public bool? IsBlocked { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
