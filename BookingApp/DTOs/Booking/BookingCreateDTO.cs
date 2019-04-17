using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class BookingCreateDTO : BookingMinimalDTO
    {
        [MaxLength(512, ErrorMessage = "Description is too long.")]
        public string Note { get; set; }

        public string CreatedUserId { get; set; }
    }
}
