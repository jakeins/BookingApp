using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    public class AppUser : IdentityUser
    {
        // Extended Properties
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        // public long? FacebookId { get; set; }
        // public string PictureUrl { get; set; }
    }
}
