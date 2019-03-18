using BookingApp.Data.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// Resource booking policy.
    /// </summary>
    public class Rule : IIdentifiable<int>, ITrackable<ApplicationUser, string>, IActivable
    {
        /// <summary>
        /// Primary identity key for the rule.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Short designation of the rule. Required.
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// Minimal usage time of a resource (minutes). Default 1m at the persistent storage.
        /// </summary>
        public int? MinTime { get; set; }

        /// <summary>
        /// Maximum usage time of a resource (minutes). Default 24h at the persistent storage.
        /// </summary>
        public int? MaxTime { get; set; }

        /// <summary>
        /// Minimal step of booking time (minutes). Default 1m at the persistent storage.
        /// </summary>
        public int? StepTime { get; set; }

        /// <summary>
        /// Time after the end of resource usage (minutes), during which the specific resource is not available for booking by anyone; "Recharge time". Default 0 at the persistent storage.
        /// </summary>
        public int? ServiceTime { get; set; }

        /// <summary>
        /// Time from the usage start (minutes), during which booking of this resource is prohibited for the current user; "Speculation timeout". Default 0 at the persistent storage.
        /// </summary>
        public int? ReuseTimeout { get; set; }

        /// <summary>
        /// Time range (minutes) used to determine how early can user book a resource, relative to the start of its usage; "Pre-Order countdown". Default 24h at the persistent storage.
        /// </summary>
        public int? PreOrderTimeLimit { get; set; }

        /// <summary>
        /// Provides deactivation functionality. Is true by default at the persistent storage.
        /// </summary>
        public bool? IsActive { get; set; }

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
