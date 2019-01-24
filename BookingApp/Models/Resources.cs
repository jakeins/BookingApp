using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public partial class Resources
    {
        public Resources()
        {
            Bookings = new HashSet<Bookings>();
        }

        [Key]
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? TreeGroupId { get; set; }
        public int RuleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public AppUser CreatedByNavigation { get; set; }
        public Rules Rule { get; set; }
        public TreeGroups TreeGroup { get; set; }
        public AppUser UpdatedByNavigation { get; set; }
        public ICollection<Bookings> Bookings { get; set; }
    }
}
