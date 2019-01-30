using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public partial class Rule
    {
        public Rule()
        {
        }

        [Key]
        [Required]
        public int RuleId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        //The time properties are nullable for having database default behaviour
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
        public int? StepTime { get; set; }
        public int? ServiceTime { get; set; }
        public int? ReuseTimeout { get; set; }
        public int? PreOrderTimeLimit { get; set; }

        #region Navigation Properties
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
