using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class BookingOwnerDTO : BookingMinimalDTO
    {
        public int BookingID { get; set; }

        [MaxLength(512, ErrorMessage = "Description is too long.")]
        public string Note { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
