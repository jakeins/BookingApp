using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class BookingUpdateDTO
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        [MaxLength(512, ErrorMessage = "Description is too long.")]
        public string Note { get; set; }
    }
}
