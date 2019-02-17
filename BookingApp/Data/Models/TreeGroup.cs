using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// Visual nesting entity, provides ability to *visually* group resources in a form of a nested set (tree).
    /// </summary>
    public class TreeGroup : IActivableEntity<int, string>
    {
        /// <summary>
        /// Primary identity key for the tree group.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Short designation of the tree group. Required.
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// Identifier of the parent tree group, where current tree group should be *visually* nested in. Optional: null means this tree group is being shown at the root level.
        /// </summary>
        public int? ParentTreeGroupId { get; set; }

        /// <summary>
        /// Identifier of a default booking rule for the nested resources during their creation. Optional: null means this tree group offers no rule as default.
        /// </summary>
        public int? DefaultRuleId { get; set; }

        /// <summary>
        /// Provides deactivation functionality. Is true by default at the persistent storage.
        /// </summary>
        public bool? IsActive { get; set; }

        #region Navigation Properties
        /// <summary>
        /// Get passed as a default booking rule for the nested resources during their creation. Nullity means this tree group offers no rule as default.
        /// </summary>
        public Rule DefaultRule { get; set; }

        /// <summary>
        /// Parent tree group, where current tree group should be *visually* nested in. Nullity means this tree group is being shown at the root level.
        /// </summary>
        public TreeGroup ParentTreeGroup { get; set; }

        /// <summary>
        /// Reverse-navigation list of all tree groups that are child relative to the current one.
        /// </summary>
        [ForeignKey("ParentTreeGroupId")]
        public IList<TreeGroup> ChildGroups { get; set; } = new List<TreeGroup>();

        /// <summary>
        /// Reverse-navigation list of all resources that are child relative to the current tree group.
        /// </summary>
        public IList<Resource> Resources { get; set; } = new List<Resource>();
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
