using BookingApp.Data.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// Valuable rentable entity that is being booked (and used) by the users over time.
    /// </summary>
    public class Resource : IIdentifiable<int>, ITrackable<ApplicationUser, string>, IActivable
    {
        /// <summary>
        /// Primary identity key for the resource.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Short designation of the resource. Required.
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// Detailed description of the resource. Optional.
        /// </summary>
        [MaxLength(512)]
        public string Description { get; set; }

        /// <summary>
        /// The identifier of the Folder at which current resource should be *visually* nested. Optional: null means this resource is being shown at the root level.
        /// </summary>
        public int? FolderId { get; set; }

        /// <summary>
        /// The identifier of the booking rule that is used to define booking policy of the current resource. Required.
        /// </summary>
        [Required]
        public int RuleId { get; set; }

        /// <summary>
        /// Provides deactivation functionality. Is true by default at the persistent storage.
        /// </summary>
        public bool? IsActive { get; set; }

        #region Navigation Properties
        /// <summary>
        /// The booking rule that is used to define booking policy of the current resource. Required.
        /// </summary>
        public Rule Rule { get; set; }

        /// <summary>
        /// The Folder at which current resource should be *visually* nested. Nullity means this resource is being shown at the root level.
        /// </summary>
        public Folder Folder { get; set; }

        /// <summary>
        /// The reverse-navigation list of all bookings of the current resource.
        /// </summary>
        public IList<Booking> Bookings { get; set; } = new List<Booking>();
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
