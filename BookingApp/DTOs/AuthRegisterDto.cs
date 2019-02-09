using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class AuthRegisterDto : AuthMinimalDto
    {
        public string UserName { get; set; }
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
