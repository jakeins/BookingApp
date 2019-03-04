using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class UserPasswordChangeDTO : UserNewPasswordDto
    {
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
    }
}
