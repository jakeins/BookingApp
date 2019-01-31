using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public partial class TreeGroup
    {
        public TreeGroup()
        {
            ChildGroups = new List<TreeGroup>();
            Resources = new List<Resource>();
        }

        [Key]
        [Required]
        public int TreeGroupId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        public int? ParentTreeGroupId { get; set; }
        public int? DefaultRuleId { get; set; }

        #region Navigation Properties
        public Rule DefaultRule { get; set; }
        public TreeGroup ParentTreeGroup { get; set; }

        [ForeignKey("ParentTreeGroupId")]
        public IList<TreeGroup> ChildGroups { get; set; }

        public IList<Resource> Resources { get; set; }
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
