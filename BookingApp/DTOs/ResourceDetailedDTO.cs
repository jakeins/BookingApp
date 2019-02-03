using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public partial class ResourceDetailedDTO : ResourceBriefDTO
    {
        [MaxLength(512, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid rule identifier.")]
        public int RuleId { get; set; }
    }
}
