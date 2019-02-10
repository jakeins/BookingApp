
namespace BookingApp.DTOs
{
    public class UserMinimalDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsActive { get; set; }
    }
}
