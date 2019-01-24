using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace BookingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        //hooked up properties; have to be nullable to overcome CLR default issue, thus be able to have real defaults in DB
        public bool? IsApproved { get; set; }
        public bool? IsActive { get; set; }
    }
}
