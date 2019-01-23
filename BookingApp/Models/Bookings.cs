using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public partial class Bookings
    {
        [Key]
        public int BookingId { get; set; }
        public int ResourceId { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public AppUser CreatedByNavigation { get; set; }
        public Resources Resource { get; set; }
        public AppUser UpdatedByNavigation { get; set; }
    }
}
