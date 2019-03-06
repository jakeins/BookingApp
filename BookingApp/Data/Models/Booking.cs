using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// Single booking occasion of a resource by the user in the specific time range. 
    /// </summary>
    public class Booking : IIdentifiable<int>, ITrackable<ApplicationUser, string>
    {
        /// <summary>
        /// Primary identity key for the booking.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Identifier of the resource being booked. Required.
        /// </summary>
        [Required]
        public int ResourceId { get; set; }

        /// <summary>
        /// Optional mid-length information about the booking. Is used to mark and differentiate alike bookings.
        /// </summary>
        [MaxLength(128)]
        public string Note { get; set; }

        /// <summary>
        /// Time of the actual resource usage start. The first usage moment. Required.
        /// See <seealso cref="CreatedTime"></seealso> for booking entry creation time.
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Planned resource usage end. Specifically, the first moment when booking is *planned* to be non-valid. Required.
        /// <para>Can be used as a factual usage end in case of <see cref="TerminationTime"></see> nullity.</para>
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Indication of the early booking termination moment. Specifically, the first moment when booking is actually no longer valid.
        /// <para>Value less or equal to <see cref="StartTime"></see> means that booking was preliminary cancelled.</para>
        /// <para>Value within <see cref="StartTime"></see> and <see cref="EndTime"></see> means that resource is freed before planned time.</para>
        /// <para>Value greater or equal to <see cref="EndTime"></see> makes no effective sense.</para>
        /// </summary>
        public DateTime? TerminationTime { get; set; }

        #region Navigation Properties
        /// <summary>
        /// The resource being booked.
        /// </summary>
        public Resource Resource { get; set; }
        #endregion

        #region User-Time tracking Properties 
        // Repeating declaration to overcome current EF Core column ordering inability

        /// <summary>
        /// Time of the current entry creation. Gets set automatically by the persistent storage.
        /// </summary>
        [Required]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Time of the current entry creation. Gets set automatically by the persistent storage.
        /// </summary>
        [Required]
        public DateTime UpdatedTime { get; set; }

        /// <summary>
        /// Identifier of the user who created current entry. Required.
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        /// <summary>
        /// Identifier of the user who updated current entry. Required.
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string UpdatedUserId { get; set; }

        /// <summary>
        /// User who created current entry.
        /// </summary>
        [ForeignKey("CreatedUserId")]
        public virtual ApplicationUser Creator { get; set; }

        /// <summary>
        /// User who updated current entry.
        /// </summary>
        [ForeignKey("UpdatedUserId")]
        public virtual ApplicationUser Updater { get; set; }
        #endregion
    }
}
