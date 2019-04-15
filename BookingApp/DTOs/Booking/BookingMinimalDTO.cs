using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class BookingMinimalDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid resource identifier.")]
        public int ResourceId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
