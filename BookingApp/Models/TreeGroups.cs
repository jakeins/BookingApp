using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public partial class TreeGroups
    {
        public TreeGroups()
        {
            InverseParentTreeGroup = new HashSet<TreeGroups>();
            Resources = new HashSet<Resources>();
        }

        [Key]
        public int TreeGroupId { get; set; }
        public string Title { get; set; }
        public int? ParentTreeGroupId { get; set; }
        public int? DefaultRuleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public AppUser CreatedByNavigation { get; set; }
        public Rules DefaultRule { get; set; }
        public TreeGroups ParentTreeGroup { get; set; }
        public AppUser UpdatedByNavigation { get; set; }
        public ICollection<TreeGroups> InverseParentTreeGroup { get; set; }
        public ICollection<Resources> Resources { get; set; }
    }
}
