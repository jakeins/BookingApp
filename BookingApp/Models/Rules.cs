using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public partial class Rules
    {
        public Rules()
        {
            Resources = new HashSet<Resources>();
            TreeGroups = new HashSet<TreeGroups>();
        }

        [Key]
        public int RuleId { get; set; }
        public string Title { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }
        public int StepTime { get; set; }
        public int ServiceTime { get; set; }
        public int ReuseTimeout { get; set; }
        public int PreOrderTimeLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public AppUser CreatedByNavigation { get; set; }
        public AppUser UpdatedByNavigation { get; set; }
        public ICollection<Resources> Resources { get; set; }
        public ICollection<TreeGroups> TreeGroups { get; set; }
    }
}
