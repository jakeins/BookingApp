using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs.User
{
    public class AdminRegisterDTO
    {
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
