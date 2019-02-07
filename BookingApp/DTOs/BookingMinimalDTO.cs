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
        int ResourceID { get; set; }

        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
    }
}
