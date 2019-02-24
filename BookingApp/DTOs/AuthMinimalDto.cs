using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class AuthMinimalDto
    {
        [EmailAddress]
        public virtual string Email { get; set; }
    }
}
