using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleDetailedDTO : RuleBasicDTO
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
