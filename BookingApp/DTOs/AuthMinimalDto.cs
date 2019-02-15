using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class AuthMinimalDto
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
