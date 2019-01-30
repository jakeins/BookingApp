using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public partial class Resource
    {
        public Resource()
        {
            Bookings = new List<Booking>();
        }

        [Key]
        [Required]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        public int? TreeGroupId { get; set; }

        [Required]
        public int RuleId { get; set; }


        #region Navigation Properties
        public Rule Rule { get; set; }
        public TreeGroup TreeGroup { get; set; }
        public IList<Booking> Bookings { get; set; }
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
