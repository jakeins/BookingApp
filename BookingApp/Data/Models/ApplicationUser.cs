using Microsoft.AspNetCore.Identity;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// The extension of the standard IdentityUser, adds signup approval and banning capabilities.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Defines whether user is approved by the administrator after sign up.
        /// <para>Null:  Newcomer,</para>
        /// <para>True:  Approved,</para>
        /// <para>False: Rejected.</para>
        /// </summary>
        public virtual bool? ApprovalStatus { get; set; }

        /// <summary>
        /// Provides ban or suspension functionality.
        /// </summary>
        public virtual bool? IsBlocked { get; set; }
    }
}
