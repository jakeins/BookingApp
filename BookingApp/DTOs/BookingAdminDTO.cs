using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class BookingAdminDTO
    {

        public int BookingID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid resource identifier.")]
        public int ResourceID { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [MaxLength(512, ErrorMessage = "Description is too long.")]
        public string Note { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        [MaxLength(450)]
        public string UpdatedUserId { get; set; }
    }
}
