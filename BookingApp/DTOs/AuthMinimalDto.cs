using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class AuthMinimalDto
    {
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
