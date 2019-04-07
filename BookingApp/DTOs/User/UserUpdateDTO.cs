using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class UserUpdateDTO
    {
        //public string Email { get; set; }

        [MinLength(3, ErrorMessage = "Title is too short.")]
        [MaxLength(64, ErrorMessage = "Title is too long.")]
        public string UserName { get; set; }

        public bool? ApprovalStatus { get; set; }
        public bool? IsBlocked { get; set; }
        //public bool EmailConfirmed { get; set; }

        public bool HasNameOnly() => ApprovalStatus == null && IsBlocked == null;
    }
}
