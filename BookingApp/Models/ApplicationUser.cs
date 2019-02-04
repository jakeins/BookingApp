using Microsoft.AspNetCore.Identity;

namespace BookingApp.Models
{
    /// <summary>
    /// The extension of the standard IdentityUser, adds signup approval and banning capabilities.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Defines whether user is approved by the administrator after sign up. Is false by default at the persistent storage.
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Provides ban or suspension functionality. Is true by default at the persistent storage.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
