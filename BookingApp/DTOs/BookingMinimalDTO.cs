using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class BookingMinimalDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid resource identifier.")]
        public int ResourceID { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
