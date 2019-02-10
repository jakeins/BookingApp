using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class BookingAdminDTO : BookingOwnerDTO
    {
        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        [MaxLength(450)]
        public string UpdatedUserId { get; set; }
    }
}
