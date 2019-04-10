namespace BookingApp.DTOs
{
    public class UserPasswordRestoreDto : UserNewPasswordDto
    {
        public string RestoreToken { get; set; }
    }
}
