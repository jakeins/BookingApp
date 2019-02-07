using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class UserGetMinimalDTO
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsActive { get; set; }
    }
}
