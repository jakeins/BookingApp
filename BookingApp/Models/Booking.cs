using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public partial class Booking
    {
        [Key]
        [Required]
        public int BookingId { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [MaxLength(128)]
        public string Note { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [DefaultValue(false)]
        public bool? IsCancelled { get; set; }

        #region Navigation Properties
        public Resource Resource { get; set; }
        #endregion

        #region User/Date modifications tracking Properties 
        // Repeating declaration to overcome current EF Core column ordering inability
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        [Required]
        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        [Required]
        [MaxLength(450)]
        public string UpdatedUserId { get; set; }

        [ForeignKey("CreatedUserId")]
        public virtual ApplicationUser Creator { get; set; }

        [ForeignKey("UpdatedUserId")]
        public virtual ApplicationUser Updater { get; set; }
        #endregion
    }
}
